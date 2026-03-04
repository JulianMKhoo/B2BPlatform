package subscriber

import (
	"context"
	"database/sql"
	"encoding/json"
	"fmt"
	"log"

	"go-chat/models"

	"github.com/redis/go-redis/v9"
)

type Subscriber struct {
	rdb *redis.Client
	db  *sql.DB
}

func New(rdb *redis.Client, db *sql.DB) *Subscriber {
	return &Subscriber{rdb: rdb, db: db}
}

func (s *Subscriber) Listen(ctx context.Context) {
	sub := s.rdb.Subscribe(ctx, "chat_events")
	ch := sub.Channel()
	fmt.Println("listening on chat_events channel")

	for msg := range ch {
		var event models.ChatEvent
		if err := json.Unmarshal([]byte(msg.Payload), &event); err != nil {
			log.Printf("failed to parse event: %v", err)
			continue
		}
		switch event.Type {
		case "workspace_created":
			s.handleWorkspaceCreated(event)
		case "workspace_deleted":
			s.handleWorkspaceDeleted(event)
		case "chat_access_granted":
			s.handleAccessGranted(event)
		case "chat_access_revoked":
			s.handleAccessRevoked(event)
		default:
			log.Printf("unknown event type: %s", event.Type)
		}
	}
}

func (s *Subscriber) handleWorkspaceCreated(e models.ChatEvent) {
	var wsID int64
	err := s.db.QueryRow(
		`INSERT INTO chat_workspaces (workspace_name, unit_id, admin_id) VALUES ($1, $2, $3)
		 ON CONFLICT (unit_id) DO NOTHING RETURNING workspace_id`,
		e.WorkspaceName, e.UnitID, e.AdminID,
	).Scan(&wsID)
	if err != nil {
		log.Printf("workspace_created failed: %v", err)
		return
	}
	// Auto-add admin as member
	_, _ = s.db.Exec(
		`INSERT INTO chat_members (workspace_id, employee_id) VALUES ($1, $2) ON CONFLICT DO NOTHING`,
		wsID, e.AdminID,
	)
	fmt.Printf("workspace created: id=%d unit=%d\n", wsID, e.UnitID)
}

func (s *Subscriber) handleWorkspaceDeleted(e models.ChatEvent) {
	_, err := s.db.Exec(
		`UPDATE chat_workspaces SET deleted_at = NOW() WHERE unit_id = $1 AND deleted_at IS NULL`,
		e.UnitID,
	)
	if err != nil {
		log.Printf("workspace_deleted failed: %v", err)
		return
	}
	fmt.Printf("workspace soft-deleted: unit=%d\n", e.UnitID)
}

func (s *Subscriber) handleAccessGranted(e models.ChatEvent) {
	_, err := s.db.Exec(
		`INSERT INTO chat_members (workspace_id, employee_id)
		 SELECT workspace_id, $2 FROM chat_workspaces WHERE unit_id = $1 AND deleted_at IS NULL
		 ON CONFLICT DO NOTHING`,
		e.UnitID, e.EmployeeID,
	)
	if err != nil {
		log.Printf("access_granted failed: %v", err)
		return
	}
	fmt.Printf("access granted: unit=%d employee=%d\n", e.UnitID, e.EmployeeID)
}

func (s *Subscriber) handleAccessRevoked(e models.ChatEvent) {
	_, err := s.db.Exec(
		`DELETE FROM chat_members WHERE employee_id = $1
		 AND workspace_id = (SELECT workspace_id FROM chat_workspaces WHERE unit_id = $2 AND deleted_at IS NULL)`,
		e.EmployeeID, e.UnitID,
	)
	if err != nil {
		log.Printf("access_revoked failed: %v", err)
		return
	}
	fmt.Printf("access revoked: unit=%d employee=%d\n", e.UnitID, e.EmployeeID)
}

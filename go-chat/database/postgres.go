package database

import (
	"database/sql"
	"fmt"
	"log"

	_ "github.com/lib/pq"
)

func Connect(dsn string) *sql.DB {
	db, err := sql.Open("postgres", dsn)
	if err != nil {
		log.Fatalf("failed to connect to postgres: %v", err)
	}
	if err := db.Ping(); err != nil {
		log.Fatalf("failed to ping postgres: %v", err)
	}
	fmt.Println("connected to postgres")
	return db
}

func Migrate(db *sql.DB) {
	queries := []string{
		`CREATE TABLE IF NOT EXISTS chat_workspaces (
			workspace_id BIGSERIAL PRIMARY KEY,
			workspace_name VARCHAR(100) NOT NULL,
			unit_id BIGINT NOT NULL UNIQUE,
			admin_id BIGINT NOT NULL,
			created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
			deleted_at TIMESTAMPTZ
		)`,
		`CREATE TABLE IF NOT EXISTS chat_members (
			member_id BIGSERIAL PRIMARY KEY,
			workspace_id BIGINT NOT NULL REFERENCES chat_workspaces(workspace_id) ON DELETE CASCADE,
			employee_id BIGINT NOT NULL,
			joined_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
			UNIQUE(workspace_id, employee_id)
		)`,
		`CREATE TABLE IF NOT EXISTS chat_messages (
			message_id BIGSERIAL PRIMARY KEY,
			workspace_id BIGINT NOT NULL REFERENCES chat_workspaces(workspace_id) ON DELETE CASCADE,
			sender_id BIGINT NOT NULL,
			message_text TEXT NOT NULL,
			sent_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
		)`,
	}
	for _, q := range queries {
		if _, err := db.Exec(q); err != nil {
			log.Fatalf("migration failed: %v", err)
		}
	}
	fmt.Println("database migrated")
}

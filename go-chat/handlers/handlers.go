package handlers

import (
	"database/sql"
	"net/http"

	"go-chat/models"

	"github.com/gin-gonic/gin"
)

type Handler struct {
	db *sql.DB
}

func New(db *sql.DB) *Handler {
	return &Handler{db: db}
}

func (h *Handler) HealthCheck(c *gin.Context) {
	c.JSON(http.StatusOK, gin.H{"status": "ok"})
}

// GetWorkspace returns workspace info by unit_id
func (h *Handler) GetWorkspace(c *gin.Context) {
	var req struct {
		UnitID int64 `json:"unit_id" binding:"required"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	var ws models.Workspace
	err := h.db.QueryRow(
		`SELECT workspace_id, workspace_name, unit_id, admin_id, created_at FROM chat_workspaces
		 WHERE unit_id = $1 AND deleted_at IS NULL`, req.UnitID,
	).Scan(&ws.WorkspaceID, &ws.WorkspaceName, &ws.UnitID, &ws.AdminID, &ws.CreatedAt)

	if err == sql.ErrNoRows {
		c.JSON(http.StatusNotFound, gin.H{"error": "workspace not found"})
		return
	}
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusOK, ws)
}

// GetMembers returns all members of a workspace
func (h *Handler) GetMembers(c *gin.Context) {
	var req struct {
		WorkspaceID int64 `json:"workspace_id" binding:"required"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	rows, err := h.db.Query(
		`SELECT member_id, workspace_id, employee_id, joined_at FROM chat_members WHERE workspace_id = $1`,
		req.WorkspaceID,
	)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	defer rows.Close()

	var members []models.Member
	for rows.Next() {
		var m models.Member
		if err := rows.Scan(&m.MemberID, &m.WorkspaceID, &m.EmployeeID, &m.JoinedAt); err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
			return
		}
		members = append(members, m)
	}
	c.JSON(http.StatusOK, gin.H{"members": members})
}

// SendMessage sends a message to a workspace
func (h *Handler) SendMessage(c *gin.Context) {
	var req struct {
		WorkspaceID int64  `json:"workspace_id" binding:"required"`
		SenderID    int64  `json:"sender_id" binding:"required"`
		MessageText string `json:"message_text" binding:"required"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// Check sender is a member
	var exists bool
	h.db.QueryRow(
		`SELECT EXISTS(SELECT 1 FROM chat_members WHERE workspace_id = $1 AND employee_id = $2)`,
		req.WorkspaceID, req.SenderID,
	).Scan(&exists)

	if !exists {
		c.JSON(http.StatusForbidden, gin.H{"error": "not a member of this workspace"})
		return
	}

	var msg models.Message
	err := h.db.QueryRow(
		`INSERT INTO chat_messages (workspace_id, sender_id, message_text) VALUES ($1, $2, $3)
		 RETURNING message_id, workspace_id, sender_id, message_text, sent_at`,
		req.WorkspaceID, req.SenderID, req.MessageText,
	).Scan(&msg.MessageID, &msg.WorkspaceID, &msg.SenderID, &msg.MessageText, &msg.SentAt)

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	c.JSON(http.StatusCreated, msg)
}

// GetMessages returns messages for a workspace
func (h *Handler) GetMessages(c *gin.Context) {
	var req struct {
		WorkspaceID int64 `json:"workspace_id" binding:"required"`
		Limit       int   `json:"limit"`
	}
	if err := c.ShouldBindJSON(&req); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	if req.Limit <= 0 {
		req.Limit = 50
	}

	rows, err := h.db.Query(
		`SELECT message_id, workspace_id, sender_id, message_text, sent_at
		 FROM chat_messages WHERE workspace_id = $1 ORDER BY sent_at DESC LIMIT $2`,
		req.WorkspaceID, req.Limit,
	)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
		return
	}
	defer rows.Close()

	var messages []models.Message
	for rows.Next() {
		var m models.Message
		if err := rows.Scan(&m.MessageID, &m.WorkspaceID, &m.SenderID, &m.MessageText, &m.SentAt); err != nil {
			c.JSON(http.StatusInternalServerError, gin.H{"error": err.Error()})
			return
		}
		messages = append(messages, m)
	}
	c.JSON(http.StatusOK, gin.H{"messages": messages})
}

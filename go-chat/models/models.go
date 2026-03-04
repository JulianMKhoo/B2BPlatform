package models

import "time"

type Workspace struct {
	WorkspaceID   int64      `json:"workspace_id"`
	WorkspaceName string     `json:"workspace_name"`
	UnitID        int64      `json:"unit_id"`
	AdminID       int64      `json:"admin_id"`
	CreatedAt     time.Time  `json:"created_at"`
	DeletedAt     *time.Time `json:"deleted_at,omitempty"`
}

type Member struct {
	MemberID    int64     `json:"member_id"`
	WorkspaceID int64     `json:"workspace_id"`
	EmployeeID  int64     `json:"employee_id"`
	JoinedAt    time.Time `json:"joined_at"`
}

type Message struct {
	MessageID   int64     `json:"message_id"`
	WorkspaceID int64     `json:"workspace_id"`
	SenderID    int64     `json:"sender_id"`
	MessageText string    `json:"message_text"`
	SentAt      time.Time `json:"sent_at"`
}

// ChatEvent is the JSON payload from .NET via Redis pub/sub
type ChatEvent struct {
	Type          string `json:"type"`
	UnitID        int64  `json:"unit_id"`
	WorkspaceName string `json:"workspace_name,omitempty"`
	AdminID       int64  `json:"admin_id,omitempty"`
	EmployeeID    int64  `json:"employee_id,omitempty"`
}

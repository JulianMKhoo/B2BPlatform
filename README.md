# B2B SaaS MVP — System Design Document

## Table of Contents

1. [Overview](#1-overview)
2. [Architecture](#2-architecture)
3. [Service Breakdown](#3-service-breakdown)
4. [Database Schema](#4-database-schema)
5. [Multi-Tenancy Strategy](#5-multi-tenancy-strategy)
6. [Inter-Service Communication](#6-inter-service-communication)
7. [Authentication & Authorization](#7-authentication--authorization)
8. [API Reference](#8-api-reference)
9. [Infrastructure & Deployment](#9-infrastructure--deployment)
10. [How to use](#10-how-to-use)

---

## 1. Overview

A multi-tenant B2B SaaS platform built with a microservices architecture. The system allows companies to register, manage employees across business units, and communicate through an integrated chat system.

**Tech Stack:**

| Layer                   | Technology                                       |
| ----------------------- | ------------------------------------------------ |
| Platform API            | .NET 10 / C# (ASP.NET Core)                      |
| Chat Service            | Go 1.24 (Gin framework)                          |
| Frontend                | Next.js 15 / React / TypeScript / Tailwind CSS   |
| Database                | PostgreSQL 15 (shared instance, separate tables) |
| Message Broker          | Redis 7 (Pub/Sub)                                |
| Caching                 | Redis 7                                          |
| Container Orchestration | Docker Compose                                   |

---

## 2. Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                          Docker Network                             │
│                                                                     │
│  ┌──────────┐     ┌──────────────┐     ┌──────────────┐            │
│  │ Frontend │────►│  .NET API    │────►│  PostgreSQL   │            │
│  │ (Next.js)│     │  (Port 5001) │     │  (Port 5432)  │            │
│  │ Port 3000│────►│              │     │               │◄───┐       │
│  └──────────┘     │              │     │  b2b_platform │    │       │
│                   │              │     └───────────────┘    │       │
│                   │              │                          │       │
│                   │              │──┐   ┌──────────────┐   │       │
│                   └──────────────┘  │   │    Redis      │   │       │
│                                     ├──►│  (Port 6379)  │   │       │
│  ┌──────────┐   ┌──────────────┐   │   │              │   │       │
│  │ Frontend │──►│  Go Chat     │───┘   │  Pub/Sub +   │   │       │
│  │          │   │  (Port 8081) │◄──────│  Cache       │   │       │
│  └──────────┘   │              │       └──────────────┘   │       │
│                  │              │──────────────────────────┘       │
│                  └──────────────┘                                   │
└─────────────────────────────────────────────────────────────────────┘
```

**Key design decisions:**

- **Service separation**: The platform API (.NET) handles business logic while the chat service (Go) handles real-time messaging, allowing independent scaling
- **Shared database**: Both services use the same PostgreSQL instance but operate on non-overlapping tables, simplifying deployment for an MVP
- **Event-driven integration**: .NET publishes domain events via Redis Pub/Sub; Go subscribes and reacts — no direct HTTP calls between services

---

## 3. Service Breakdown

### 3.1 .NET Platform API

The main business platform handling company management, employee lifecycle, authentication, and event publishing.

**Project Structure:**

```
dotnet-api/
├── dotnet-api.sln
├── B2BPlatform.API/                    # Entry point, controllers, Dockerfile
│   ├── Program.cs                      # DI setup, CORS, Swagger, auto-migration
│   ├── Controllers/
│   │   ├── AuthenticationController.cs # Owner/employee login & registration
│   │   ├── CompanyController.cs        # Company CRUD
│   │   ├── CompanyOwnerController.cs   # Owner management
│   │   ├── EmployeeController.cs       # Employee + EmployeeData CRUD
│   │   └── BusinessUnitController.cs   # BU management + event publishing
│   └── Handlers/
│       └── GlobalExceptionHandler.cs   # Centralized error handling
├── B2BPlatform.Shared/                 # Entities, interfaces, DTOs
│   ├── Entities/                       # EF Core entity classes
│   ├── Interfaces/
│   │   ├── Repositories/              # Repository contracts
│   │   └── Services/                  # Service contracts
│   └── Models/
│       ├── Commons/                   # Base response types
│       └── Dto/                       # Request/response DTOs
├── B2BPlatform.Infrastructure/         # Data access layer
│   ├── Contexts/AppDbContext.cs        # EF Core DbContext
│   ├── Repositories/                  # Repository implementations
│   └── Services/                      # Redis cache & event publisher
├── B2BPlatform.Infrastructure.IoC/     # Dependency injection registration
└── B2BPlatform.Services.*/             # Service layer implementations
    ├── Authentications/
    ├── BusinessUnit/
    ├── Chats/
    ├── Company/
    ├── CompanyOwner/
    └── Employees/
```

**Startup pipeline** (`Program.cs`):

1. Register DI services (repositories, services, Redis connections)
2. Configure CORS (allow all origins for MVP)
3. Configure Swagger for API documentation
4. Auto-migrate database on startup (`db.Database.Migrate()`)
5. Map controllers

### 3.2 Go Chat Service

A standalone microservice handling chat workspaces, membership, and messaging. It reacts to domain events from .NET via Redis Pub/Sub.

**Project Structure:**

```
go-chat/
├── main.go              # Entry point, Gin router setup
├── config/config.go     # Environment configuration
├── database/
│   ├── postgres.go      # DB connection + auto-migration (CREATE TABLE IF NOT EXISTS)
│   └── redis.go         # Redis client setup
├── handlers/handlers.go # HTTP route handlers
├── models/models.go     # Data models + ChatEvent struct
├── subscriber/subscriber.go  # Redis Pub/Sub listener
├── Dockerfile           # Multi-stage build (golang:1.24-alpine)
├── go.mod
└── go.sum
```

**Auto-migration on startup creates 3 tables:**

```sql
chat_workspaces  (workspace_id, workspace_name, unit_id UNIQUE, admin_id, created_at, deleted_at)
chat_members     (member_id, workspace_id FK, employee_id, joined_at, UNIQUE(workspace_id, employee_id))
chat_messages    (message_id, workspace_id FK, sender_id, message_text, sent_at)
```

### 3.3 React Frontend

A Next.js 15 application with App Router providing the complete user interface.

**Project Structure:**

```
frontend/
├── app/
│   ├── layout.tsx                      # Root layout, <AuthProvider> wrapper
│   ├── page.tsx                        # Redirect → /auth/login
│   ├── auth/
│   │   ├── login/page.tsx              # Two-tab login (Owner / Employee)
│   │   └── register/page.tsx           # Company onboarding
│   └── dashboard/
│       ├── layout.tsx                  # Auth guard, nav, "No BU" state
│       ├── page.tsx                    # Overview (owner/admin only)
│       ├── employees/page.tsx          # Employee management
│       ├── business-units/page.tsx     # BU management
│       ├── chat/page.tsx               # Chat interface + access control
│       └── profile/page.tsx            # Account info + change password
├── lib/
│   ├── auth-context.tsx                # AuthProvider + useAuth hook
│   ├── api.ts                          # All API calls (.NET + Go)
│   └── types.ts                        # TypeScript interfaces
└── Dockerfile
```

**Page access by role:**

| Page           | Owner                 | Admin                | Employee           |
| -------------- | --------------------- | -------------------- | ------------------ |
| Overview       | Yes                   | Yes                  | Redirected to Chat |
| Employees      | Yes (full control)    | Yes (limited delete) | No                 |
| Business Units | Yes (create + delete) | Yes (create only)    | No                 |
| Chat           | Yes (all workspaces)  | Yes (all workspaces) | Yes (own BU only)  |
| Profile        | Yes                   | Yes                  | Yes                |

---

## 4. Database Schema

### ER Diagram

See `dotnet-api/table.jpg` for the full ER diagram.

### .NET Platform Tables (managed by EF Core)

#### Company

| Column          | Type      | Constraints            |
| --------------- | --------- | ---------------------- |
| company_id      | BIGINT    | PK, auto-increment     |
| company_name    | VARCHAR   | NOT NULL               |
| company_address | VARCHAR   | NOT NULL               |
| contact_number  | VARCHAR   | NOT NULL               |
| contract_number | INTEGER   |                        |
| created_at      | TIMESTAMP |                        |
| updated_at      | TIMESTAMP | nullable               |
| deleted_at      | TIMESTAMP | nullable (soft delete) |

#### CompanyOwner

| Column           | Type      | Constraints            |
| ---------------- | --------- | ---------------------- |
| company_owner_id | BIGINT    | PK, auto-increment     |
| company_id       | BIGINT    | FK → Company, NOT NULL |
| password         | VARCHAR   | NOT NULL               |
| created_at       | TIMESTAMP |                        |
| updated_at       | TIMESTAMP | nullable               |
| deleted_at       | TIMESTAMP | nullable               |

#### Employee

| Column      | Type      | Constraints            |
| ----------- | --------- | ---------------------- |
| employee_id | BIGINT    | PK, auto-increment     |
| first_name  | VARCHAR   | NOT NULL               |
| last_name   | VARCHAR   | NOT NULL               |
| position    | SMALLINT  | NOT NULL               |
| company_id  | BIGINT    | FK → Company, NOT NULL |
| created_at  | TIMESTAMP |                        |
| updated_at  | TIMESTAMP | nullable               |
| deleted_at  | TIMESTAMP | nullable               |

#### EmployeeData

| Column           | Type      | Constraints                    |
| ---------------- | --------- | ------------------------------ |
| employee_data_id | BIGINT    | PK, auto-increment             |
| email            | VARCHAR   | NOT NULL, UNIQUE               |
| password         | VARCHAR   | NOT NULL                       |
| team_name        | VARCHAR   | NOT NULL                       |
| role             | SMALLINT  | NOT NULL (0=Employee, 1=Admin) |
| employee_id      | BIGINT    | FK → Employee, NOT NULL        |
| unit_id          | BIGINT    | FK → BusinessUnit, NOT NULL    |
| created_at       | TIMESTAMP |                                |
| updated_at       | TIMESTAMP | nullable                       |
| deleted_at       | TIMESTAMP | nullable                       |
|                  |           | UNIQUE(employee_id, unit_id)   |

#### BusinessUnit

| Column             | Type      | Constraints                 |
| ------------------ | --------- | --------------------------- |
| business_unit_id   | BIGINT    | PK, auto-increment          |
| business_unit_name | VARCHAR   | NOT NULL                    |
| company_id         | BIGINT    | FK → Company, NOT NULL      |
| company_owner_id   | BIGINT    | FK → CompanyOwner, NOT NULL |
| created_at         | TIMESTAMP |                             |
| updated_at         | TIMESTAMP | nullable                    |
| deleted_at         | TIMESTAMP | nullable                    |

### Go Chat Tables (managed by raw SQL migrations)

#### chat_workspaces

| Column         | Type         | Constraints      |
| -------------- | ------------ | ---------------- |
| workspace_id   | BIGSERIAL    | PK               |
| workspace_name | VARCHAR(255) | NOT NULL         |
| unit_id        | BIGINT       | NOT NULL, UNIQUE |
| admin_id       | BIGINT       | NOT NULL         |
| created_at     | TIMESTAMP    | DEFAULT NOW()    |
| deleted_at     | TIMESTAMP    | nullable         |

#### chat_members

| Column       | Type      | Constraints                       |
| ------------ | --------- | --------------------------------- |
| member_id    | BIGSERIAL | PK                                |
| workspace_id | BIGINT    | FK → chat_workspaces (CASCADE)    |
| employee_id  | BIGINT    | NOT NULL                          |
| joined_at    | TIMESTAMP | DEFAULT NOW()                     |
|              |           | UNIQUE(workspace_id, employee_id) |

#### chat_messages

| Column       | Type      | Constraints                    |
| ------------ | --------- | ------------------------------ |
| message_id   | BIGSERIAL | PK                             |
| workspace_id | BIGINT    | FK → chat_workspaces (CASCADE) |
| sender_id    | BIGINT    | NOT NULL                       |
| message_text | TEXT      | NOT NULL                       |
| sent_at      | TIMESTAMP | DEFAULT NOW()                  |

### Relationships

```
Company (1) ──── (N) Employee
Company (1) ──── (N) CompanyOwner
Company (1) ──── (N) BusinessUnit
CompanyOwner (1) ──── (N) BusinessUnit          (owner manages BUs)
Employee (1) ──── (1) EmployeeData              (credentials & assignment)
BusinessUnit (1) ──── (N) EmployeeData          (employees assigned to BUs)
BusinessUnit (1) ──── (1) chat_workspaces        (via unit_id)
chat_workspaces (1) ──── (N) chat_members
chat_workspaces (1) ──── (N) chat_messages
```

---

## 5. Multi-Tenancy Strategy

### Approach: Shared Database, Row-Level Isolation

All tenants (companies) share a single PostgreSQL database. Tenant isolation is enforced at the **application level** through `company_id` filtering on every query.

```
┌─────────────────────────────────────────┐
│            PostgreSQL (b2b_platform)     │
│                                         │
│  Company A (company_id=1)               │
│    ├── Employees where company_id=1     │
│    ├── BusinessUnits where company_id=1 │
│    └── CompanyOwner where company_id=1  │
│                                         │
│  Company B (company_id=2)               │
│    ├── Employees where company_id=2     │
│    ├── BusinessUnits where company_id=2 │
│    └── CompanyOwner where company_id=2  │
└─────────────────────────────────────────┘
```

**How isolation is enforced:**

1. **Employee List**: Filtered by `e.CompanyId == request.Id` — employees only see their own company's staff
2. **Business Unit List**: Filtered by `CompanyId` — each company sees only its own BUs
3. **Chat Workspaces**: Linked to BUs via `unit_id`, which are company-scoped — chat isolation is inherited from BU isolation
4. **Frontend**: The `companyId` is stored in auth context at login and passed with every API call

**Why this approach for an MVP:**

- Single database simplifies deployment and operations
- No cross-tenant query risk since every query includes `company_id`
- Easy to migrate to database-per-tenant later if needed (the `company_id` FK pattern makes it straightforward)

### Soft Delete Pattern

All entities use a `deleted_at` timestamp column instead of physical deletion:

- **Read queries** filter with `WHERE deleted_at IS NULL`
- **Delete operations** set `deleted_at = DateTime.UtcNow`
- **Cascading soft deletes**: Deleting an Employee also soft-deletes its EmployeeData; deleting a BusinessUnit triggers a `workspace_deleted` event

This preserves data for auditing and allows recovery.

---

## 6. Inter-Service Communication

### Pattern: Event-Driven via Redis Pub/Sub

The .NET API publishes domain events to a Redis channel (`chat_events`). The Go Chat Service subscribes and reacts to these events autonomously. There are no synchronous HTTP calls between services.

```
┌──────────────┐    Publish JSON     ┌─────────┐    Subscribe     ┌──────────────┐
│   .NET API   │ ──────────────────► │  Redis  │ ───────────────► │  Go Chat     │
│              │   channel:          │ Pub/Sub │                  │  Service     │
│  (Producer)  │   "chat_events"     │         │                  │  (Consumer)  │
└──────────────┘                     └─────────┘                  └──────────────┘
```

### Event Types

| Event                 | Trigger in .NET                                           | Go Chat Action                                          |
| --------------------- | --------------------------------------------------------- | ------------------------------------------------------- |
| `workspace_created`   | Company registration (default BU) or new BU created       | `INSERT INTO chat_workspaces`, auto-add admin as member |
| `workspace_deleted`   | BU soft-deleted                                           | `SET deleted_at = NOW()` on chat_workspaces             |
| `chat_access_granted` | Employee created via `CreateEmployeeFull` or manual grant | `INSERT INTO chat_members`                              |
| `chat_access_revoked` | Manual revoke by owner/admin                              | `DELETE FROM chat_members`                              |

### Event Payload Format

```json
// workspace_created
{ "type": "workspace_created", "unit_id": 5, "workspace_name": "Engineering Chat", "admin_id": 1 }

// workspace_deleted
{ "type": "workspace_deleted", "unit_id": 5 }

// chat_access_granted
{ "type": "chat_access_granted", "unit_id": 5, "employee_id": 12 }

// chat_access_revoked
{ "type": "chat_access_revoked", "unit_id": 5, "employee_id": 12 }
```

### Event Flow Examples

**Company Registration:**

```
Owner registers → .NET creates Company + Owner + Employee + Default BU + EmployeeData
                → Publishes workspace_created (workspace="General", admin=ownerId)
                → Go creates chat_workspaces row + adds admin as first member
```

**New Employee Onboarding:**

```
Owner creates employee → .NET creates Employee + EmployeeData (transactional)
                       → Publishes chat_access_granted (unit_id, employee_id)
                       → Go inserts into chat_members
                       → Employee can now send messages in their BU workspace
```

**BU Deletion:**

```
Owner deletes BU → .NET soft-deletes BusinessUnit
                 → Publishes workspace_deleted (unit_id)
                 → Go soft-deletes chat_workspaces row
                 → Affected employees see "No Business Unit" state on next login/refresh
```

### Why Redis Pub/Sub (not HTTP)?

- **Loose coupling**: Go service doesn't need to know .NET's API structure (and vice versa)
- **Fire-and-forget**: .NET doesn't wait for Go to process — no latency impact on the main API
- **Resilience**: If Go is temporarily down, .NET operations still succeed (events are lost, but acceptable for MVP)
- **Simplicity**: Redis is already in the stack for caching, so no additional infrastructure needed

---

## 7. Authentication & Authorization

### Authentication Flow

The system uses application-level authentication without JWT tokens. Auth state is managed client-side via React Context + localStorage.

```
┌──────────┐    POST /auth/login     ┌──────────┐    Query DB    ┌──────────┐
│ Frontend │ ──────────────────────► │ .NET API │ ─────────────► │ Postgres │
│          │ ◄────────────────────── │          │ ◄───────────── │          │
│          │    { id, companyId,     │          │   Compare pwd  │          │
│          │      unitId, role }     │          │                │          │
│          │                         └──────────┘                └──────────┘
│          │
│  Stores in localStorage:
│  { ownerId, companyId, employeeId, unitId, role, employeeRole, ... }
│  Session expires after 2 days
└──────────┘
```

**Two login flows:**

1. **Owner Login**: Numeric Owner ID + password → returns `{ ownerId, companyId }`
2. **Employee Login**: Email + password → returns `{ employeeId, companyId, unitId, role, firstName, lastName }`

### Role Hierarchy

```
Company Owner (role: "owner")
    │
    ├── Full control: all CRUD, delete BUs, manage all employees
    ├── Can assign/change employee roles and BU assignments
    └── Can grant/revoke chat access
         │
Admin (role: "employee", employeeRole: 1)
    │
    ├── Can view/create employees and BUs
    ├── Can delete regular employees (not other admins)
    ├── Cannot delete BUs (owner-only)
    └── Has access to management dashboard
         │
Employee (role: "employee", employeeRole: 0)
    │
    ├── Chat access only (own BU workspace)
    ├── Profile & password management
    └── No access to management pages
```

### Deleted BU Handling

When a BU is deleted, affected employees experience a "No Business Unit" state:

1. **On login**: `EmployeeLogin` includes the BusinessUnit entity — if `BusinessUnit.DeletedAt != null`, returns `unitId = 0`
2. **On dashboard load**: Layout checks if `unitId === 0`, calls `POST /check_unit` to see if reassignment happened
3. **If still unassigned**: Full-page message: "Your business unit has been removed. Please wait for your company owner to reassign you."
4. **Owner reassigns**: Updates `EmployeeData.UnitId` via dropdown → employee refreshes → normal dashboard restored

---

## 8. API Reference

All endpoints use **POST** method. All responses include a `status` object with `code` and `message`.

### .NET Platform API (Port 5001)

#### Authentication (`/api/authentication/v1/`)

| Endpoint                             | Request Body                                       | Response                                               |
| ------------------------------------ | -------------------------------------------------- | ------------------------------------------------------ |
| `company_owner/auth/register`        | `{ companyName, companyAddress?, contactNumber? }` | `{ ownerId, defaultPassword }`                         |
| `company_owner/auth/login`           | `{ id, password }`                                 | `{ id, companyId }`                                    |
| `company_owner/auth/change_password` | `{ id, currentPassword, newPassword }`             | `{ status }`                                           |
| `employee/auth/login`                | `{ email, password }`                              | `{ id, companyId, unitId, role, firstName, lastName }` |
| `employee/auth/change_password`      | `{ employeeId, currentPassword, newPassword }`     | `{ status }`                                           |

#### Employees (`/api/employee/v1/`)

| Endpoint      | Request Body                                                        | Response                                                                                     |
| ------------- | ------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| `get_all`     | `{ id }` (companyId)                                                | `{ employee: [{ id, firstName, lastName, position, role, email, employeeDataId, unitId }] }` |
| `get`         | `{ id }`                                                            | `{ employee: { ... } }`                                                                      |
| `create_full` | `{ firstName, lastName, position, email, unitId, companyId, role }` | `{ employeeId, generatedPassword }`                                                          |
| `data/update` | `{ id, email?, password?, teamName?, role?, unitId? }`              | `{ status }`                                                                                 |
| `delete`      | `{ id }`                                                            | `{ status }` (soft-deletes Employee + EmployeeData)                                          |
| `check_unit`  | `{ employeeId }`                                                    | `{ unitId }` (0 if BU deleted)                                                               |

#### Business Units (`/api/businessunit/v1/`)

| Endpoint  | Request Body                                      | Response                                              |
| --------- | ------------------------------------------------- | ----------------------------------------------------- |
| `get_all` | `{ companyId }`                                   | `{ businessUnits: [{ id, businessUnitName }] }`       |
| `get`     | `{ id }`                                          | `{ id, businessUnitName, companyId, companyOwnerId }` |
| `insert`  | `{ businessUnitName, companyId, companyOwnerId }` | `{ status }` + publishes `workspace_created`          |
| `delete`  | `{ id }`                                          | `{ status }` + publishes `workspace_deleted`          |

#### Company (`/api/company/v1/`)

| Endpoint | Request Body                                      | Response                                          |
| -------- | ------------------------------------------------- | ------------------------------------------------- |
| `get`    | `{ id }`                                          | `{ companyName, companyAddress, contractNumber }` |
| `insert` | `{ companyName, companyAddress, contractNumber }` | `{ status }`                                      |

### Go Chat Service (Port 8081)

| Endpoint                          | Request Body                                | Response                                                           |
| --------------------------------- | ------------------------------------------- | ------------------------------------------------------------------ |
| `GET /health`                     | —                                           | Health check                                                       |
| `POST /api/chat/v1/workspace/get` | `{ unit_id }`                               | `{ workspace_id, workspace_name }`                                 |
| `POST /api/chat/v1/members/get`   | `{ workspace_id }`                          | `{ members: [{ member_id, employee_id }] }`                        |
| `POST /api/chat/v1/message/send`  | `{ workspace_id, sender_id, message_text }` | `201` or `403` (not a member)                                      |
| `POST /api/chat/v1/messages/get`  | `{ workspace_id, limit? }`                  | `{ messages: [{ message_id, sender_id, message_text, sent_at }] }` |

---

## 9. Infrastructure & Deployment

### Docker Compose Setup

```yaml
services:
  postgres: # PostgreSQL 15 — shared by .NET and Go
  redis: # Redis 7 — Pub/Sub + caching
  dotnet-api: # .NET 10 API on port 5001
  go-chat: # Go Chat Service on port 8081
  frontend: # Next.js on port 3000
```

| Service    | Port        | Depends On                  |
| ---------- | ----------- | --------------------------- |
| PostgreSQL | 5432        | —                           |
| Redis      | 6379        | —                           |
| .NET API   | 5001 → 8080 | PostgreSQL (healthy), Redis |
| Go Chat    | 8081        | PostgreSQL, Redis           |
| Frontend   | 3000        | .NET API, Go Chat           |

**Network**: All services on a shared `b2b_network` bridge network.

### Build Strategy

Both backend services use **multi-stage Docker builds** to minimize image size:

- **.NET**: `mcr.microsoft.com/dotnet/sdk` → build → `mcr.microsoft.com/dotnet/aspnet` runtime
- **Go**: `golang:1.24-alpine` → build → `alpine:latest` runtime

### Startup Sequence

1. PostgreSQL starts and reports healthy
2. Redis starts
3. .NET API starts → runs EF Core auto-migration → creates/updates platform tables
4. Go Chat starts → runs SQL auto-migration → creates chat tables
5. Go Chat starts Redis subscriber goroutine (listens on `chat_events`)
6. Frontend starts → ready to serve

### Environment Variables

| Service  | Variable                   | Purpose                      |
| -------- | -------------------------- | ---------------------------- |
| .NET     | `ConnectionStrings__WRITE` | PostgreSQL write connection  |
| .NET     | `ConnectionStrings__READ`  | PostgreSQL read connection   |
| .NET     | `ConnectionStrings__REDIS` | Redis connection             |
| Go       | `DB_DSN`                   | PostgreSQL connection string |
| Go       | `REDIS_URL`                | Redis host:port              |
| Frontend | `NEXT_PUBLIC_API_URL`      | .NET API base URL            |
| Frontend | `NEXT_PUBLIC_CHAT_API_URL` | Go Chat base URL             |

---

## 10. How to use

add env file with following lines in frontend folder name it `.env.local`

```
NEXT_PUBLIC_API_URL=http://localhost:5001
NEXT_PUBLIC_CHAT_API_URL=http://localhost:8081
```

cd back to the root folder then

```
docker compose -f docker-compose.yaml up -d
```

finally, open `localhost:3000` on browser

---

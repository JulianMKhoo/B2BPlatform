import type {
  EmployeeLoginResponse,
  CreateEmployeeFullRequest,
  CreateEmployeeFullResponse,
  GetBusinessUnitListResponse,
  BaseResponse,
} from "./types";

const API = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001";
const CHAT_API = process.env.NEXT_PUBLIC_CHAT_API_URL || "http://localhost:8081";

async function post<T>(base: string, path: string, body?: unknown): Promise<T> {
  const res = await fetch(`${base}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: body ? JSON.stringify(body) : undefined,
  });
  return res.json();
}

// .NET Auth
export const auth = {
  register: (data: { companyName: string; companyAddress?: string; contactNumber?: string }) =>
    post<{ ownerId: number; defaultPassword: string; status?: { code: string; message: string } }>(
      API, "/api/authentication/v1/company_owner/auth/register", data
    ),
  ownerLogin: (data: { id: number; password: string }) =>
    post<{ id: number; companyId: number; status?: { code: string; message: string } }>(
      API, "/api/authentication/v1/company_owner/auth/login", data
    ),
  employeeLogin: (data: { email: string; password: string }) =>
    post<EmployeeLoginResponse>(
      API, "/api/authentication/v1/employee/auth/login", data
    ),
  changeOwnerPassword: (data: { id: number; currentPassword: string; newPassword: string }) =>
    post<BaseResponse>(
      API, "/api/authentication/v1/company_owner/auth/change_password", data
    ),
  changeEmployeePassword: (data: { employeeId: number; currentPassword: string; newPassword: string }) =>
    post<BaseResponse>(
      API, "/api/authentication/v1/employee/auth/change_password", data
    ),
};

// .NET Company
export const company = {
  get: (id: number) => post(API, "/api/company/v1/get", { id }),
  insert: (data: { companyName: string; companyAddress: string; contractNumber: string }) =>
    post(API, "/api/company/v1/insert", data),
};

// .NET Employees
export const employees = {
  list: (companyId: number) => post(API, "/api/employee/v1/get_all", { id: companyId }),
  get: (id: number) => post(API, "/api/employee/v1/get", { id }),
  insert: (data: { firstName: string; lastName: string; position?: number; companyId?: number }) =>
    post(API, "/api/employee/v1/insert", data),
  delete: (id: number) => post(API, "/api/employee/v1/delete", { id }),
  createFull: (data: CreateEmployeeFullRequest) =>
    post<CreateEmployeeFullResponse>(API, "/api/employee/v1/create_full", data),
  updateRole: (employeeDataId: number, role: number) =>
    post(API, "/api/employee/v1/data/update", { id: employeeDataId, role }),
  updateUnit: (employeeDataId: number, unitId: number) =>
    post(API, "/api/employee/v1/data/update", { id: employeeDataId, unitId }),
  checkUnit: (employeeId: number) =>
    post<{ unitId: number; status?: { code: string; message: string } }>(
      API, "/api/employee/v1/check_unit", { employeeId }
    ),
};

// .NET Business Units
export const businessUnits = {
  list: (companyId: number) =>
    post<GetBusinessUnitListResponse>(API, "/api/businessunit/v1/get_all", { companyId }),
  get: (id: number) =>
    post<{ id: number; businessUnitName: string; companyId: number; companyOwnerId: number; status?: { code: string; message: string } }>(
      API, "/api/businessunit/v1/get", { id }
    ),
  insert: (data: { businessUnitName: string; companyId: number; companyOwnerId: number }) =>
    post(API, "/api/businessunit/v1/insert", data),
  delete: (id: number) => post(API, "/api/businessunit/v1/delete", { id }),
};

// .NET Chat access control
export const chatAccess = {
  grant: (data: { unitId: number; employeeId: number }) =>
    post(API, "/api/chat/v1/access/grant", data),
  revoke: (data: { unitId: number; employeeId: number }) =>
    post(API, "/api/chat/v1/access/revoke", data),
};

// Go Chat Service
export const chat = {
  getWorkspace: (unitId: number) =>
    post<{ workspace_id: number; workspace_name: string } | { error: string }>(
      CHAT_API, "/api/chat/v1/workspace/get", { unit_id: unitId }
    ),
  getMembers: (workspaceId: number) =>
    post<{ members: { member_id: number; employee_id: number }[] }>(
      CHAT_API, "/api/chat/v1/members/get", { workspace_id: workspaceId }
    ),
  sendMessage: (data: { workspace_id: number; sender_id: number; message_text: string }) =>
    post(CHAT_API, "/api/chat/v1/message/send", data),
  getMessages: (workspaceId: number, limit = 50) =>
    post<{ messages: { message_id: number; sender_id: number; message_text: string; sent_at: string }[] | null }>(
      CHAT_API, "/api/chat/v1/messages/get", { workspace_id: workspaceId, limit }
    ),
};

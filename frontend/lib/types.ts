// Shared status types matching .NET ServiceBaseResponse
export interface ServiceStatus {
  code: string;
  message: string;
  error?: { errorCode: string; errorMessage: string };
}

export interface BaseResponse {
  status?: ServiceStatus;
}

// Auth
export interface CompanyOwnerRegisterRequest {
  companyName: string;
  companyAddress?: string;
  contactNumber?: string;
}

export interface CompanyOwnerRegisterResponse extends BaseResponse {
  ownerId: number;
  defaultPassword: string;
}

export interface CompanyOwnerLoginRequest {
  id: number;
  password: string;
}

export interface CompanyOwnerLoginResponse extends BaseResponse {
  id: number;
  companyId: number;
}

// Company
export interface Company {
  id: number;
  companyName: string;
  companyAddress: string;
  contractNumber: string;
}

export interface GetCompanyResponse extends BaseResponse, Company {}

// Employee
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  position?: number;
  companyId?: number;
  role?: number;
  email?: string;
  employeeDataId?: number;
  unitId?: number;
}

export interface GetEmployeeListResponse extends BaseResponse {
  employee?: Employee[];
}

export interface InsertEmployeeRequest {
  firstName: string;
  lastName: string;
  position?: number;
  companyId?: number;
}

// Business Unit
export interface BusinessUnit {
  id: number;
  businessUnitName: string;
  companyId: number;
  companyOwnerId: number;
}

export interface GetBusinessUnitResponse extends BaseResponse, BusinessUnit {}

export interface BusinessUnitItem {
  id: number;
  businessUnitName: string;
}

export interface GetBusinessUnitListResponse extends BaseResponse {
  businessUnits?: BusinessUnitItem[];
}

export interface InsertBusinessUnitRequest {
  businessUnitName: string;
  companyId: number;
  companyOwnerId: number;
}

// Chat (Go service)
export interface ChatWorkspace {
  workspace_id: number;
  workspace_name: string;
  unit_id: number;
  admin_id: number;
}

export interface ChatMember {
  member_id: number;
  workspace_id: number;
  employee_id: number;
}

export interface ChatMessage {
  message_id: number;
  workspace_id: number;
  sender_id: number;
  message_text: string;
  sent_at: string;
}

// Chat access (via .NET → Redis → Go)
export interface GrantChatAccessRequest {
  unitId: number;
  employeeId: number;
}

// Employee Login
export interface EmployeeLoginRequest {
  email: string;
  password: string;
}

export interface EmployeeLoginResponse extends BaseResponse {
  id: number;
  companyId: number;
  unitId: number;
  role: number;
  firstName: string;
  lastName: string;
}

// Create Employee Full
export interface CreateEmployeeFullRequest {
  firstName: string;
  lastName: string;
  position: number;
  email: string;
  unitId: number;
  companyId: number;
  role: number;
}

export interface CreateEmployeeFullResponse extends BaseResponse {
  employeeId: number;
  generatedPassword: string;
}

// Change Password
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// Enums
export const POSITION_LABELS: Record<number, string> = {
  1: "Developer",
  2: "Financial",
  3: "Manager",
  4: "Designer",
  5: "Analyst",
};

export const ROLE_LABELS: Record<number, string> = {
  0: "Employee",
  1: "Admin",
};

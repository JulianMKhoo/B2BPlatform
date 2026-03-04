using B2BPlatform.Shared.Models.Commons;

namespace B2BPlatform.Shared.Models.Dto;

// Company Owner - Register (Company Onboarding)
public record CompanyOwnerRegisterRequest
{
    public required string CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? ContactNumber { get; set; }
}

public record CompanyOwnerRegisterResponse : ServiceBaseResponse
{
    public long OwnerId { get; set; }
    public string? DefaultPassword { get; set; }
}

// Company Owner - Login (by owner ID + password)
public record CompanyOwnerLoginRequest
{
    public long Id { get; set; }
    public required string Password { get; set; }
}

public record CompanyOwnerLoginResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}

// Employee - Login
public record EmployeeLoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public record EmployeeLoginResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long UnitId { get; set; }
    public short Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

// Employee - Register
public record EmployeeRegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public short Position { get; set; }
}

public record EmployeeRegisterResponse : ServiceBaseResponse;

// Change Password - Owner
public record ChangeOwnerPasswordRequest
{
    public long Id { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public record ChangeOwnerPasswordResponse : ServiceBaseResponse;

// Change Password - Employee
public record ChangeEmployeePasswordRequest
{
    public long EmployeeId { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public record ChangeEmployeePasswordResponse : ServiceBaseResponse;

// Repository-level data records
public record CompanyOwnerRegisterData
{
    public long OwnerId { get; set; }
    public string? DefaultPassword { get; set; }
}

public record CompanyOwnerLoginData
{
    public long OwnerId { get; set; }
    public long CompanyId { get; set; }
}

public record EmployeeLoginData
{
    public long EmployeeId { get; set; }
    public long CompanyId { get; set; }
    public long UnitId { get; set; }
    public short Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

using B2BPlatform.Shared.Models.Commons;

namespace B2BPlatform.Shared.Models.Dto;

public record GetEmployeeListRequest
{
    public long Id { get; set; }
}

public record GetEmployeeListResponse : ServiceBaseResponse
{
    public List<Employee>? Employee { get; set; }
}

public record GetEmployeeRequest
{
    public long Id { get; set; }
}

public record GetEmployeeResponse : ServiceBaseResponse
{
    public Employee? Employee { get; set; }
}

public record Employee
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public short? Position { get; set; }
    public long? CompanyId { get; set; }
    public short? Role { get; set; }
    public string? Email { get; set; }
    public long? EmployeeDataId { get; set; }
    public long? UnitId { get; set; }
}

public record InsertEmployeeRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public short? Position { get; set; }
    public long? CompanyId { get; set; }
}
public record InsertEmployeeResponse: ServiceBaseResponse;

public record UpdateEmployeeRequest
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public short? Position { get; set; }
    public long? CompanyId { get; set; }
}
public record UpdateEmployeeResponse: ServiceBaseResponse;

public record DeleteEmployeeRequest
{
    public long Id { get; set; }
}
public record DeleteEmployeeResponse: ServiceBaseResponse;

public record GetEmployeeDataRequest
{
    public long Id { get; set; }
}

public record GetEmployeeDataResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public required string Email { get; set; }
    public string? TeamName { get; set; }
    public short? Role { get; set; }
}

public record InsertEmployeeDataRequest
{
    public long EmployeeId { get; set; }
    public long UnitId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? TeamName { get; set; }
    public short? Role { get; set; }
}
public record InsertEmployeeDataResponse: ServiceBaseResponse;

public record UpdateEmployeeDataRequest
{
    public long Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? TeamName { get; set; }
    public short? Role { get; set; }
    public long? UnitId { get; set; }
}
public record UpdateEmployeeDataResponse: ServiceBaseResponse;

public record CheckEmployeeUnitRequest
{
    public long EmployeeId { get; set; }
}

public record CheckEmployeeUnitResponse : ServiceBaseResponse
{
    public long UnitId { get; set; }
}

public record DeleteEmployeeDataRequest
{
    public long Id { get; set; }
}
public record DeleteEmployeeDataResponse: ServiceBaseResponse;

// Create Employee Full (Employee + EmployeeData + auto-gen password + chat access)
public record CreateEmployeeFullRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public short Position { get; set; }
    public required string Email { get; set; }
    public long UnitId { get; set; }
    public long CompanyId { get; set; }
    public short Role { get; set; }
}

public record CreateEmployeeFullResponse : ServiceBaseResponse
{
    public long EmployeeId { get; set; }
    public string? GeneratedPassword { get; set; }
}

public record CreateEmployeeFullData
{
    public long EmployeeId { get; set; }
    public string? GeneratedPassword { get; set; }
}
using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Shared.Entities;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context, IChatEventPublisher chatEventPublisher) : IEmployeeRepository
{
    public async Task<RepositoryBaseResponse<GetEmployeeListResponse>> GetEmployeeList(GetEmployeeListRequest request)
    {
        var employees = await context.Employees
            .Where(e => e.CompanyId == request.Id && e.DeletedAt == null)
            .GroupJoin(
                context.EmployeeData.Where(ed => ed.DeletedAt == null),
                e => e.Id,
                ed => ed.EmployeeId,
                (e, edGroup) => new { e, ed = edGroup.FirstOrDefault() }
            )
            .Where(x => x.ed == null || !x.ed.Email.StartsWith("owner@"))
            .Select(x => new Employee
            {
                Id = x.e.Id,
                FirstName = x.e.FirstName,
                LastName = x.e.LastName,
                Position = x.e.Position,
                CompanyId = x.e.CompanyId,
                Role = x.ed != null ? x.ed.Role : (short?)null,
                Email = x.ed != null ? x.ed.Email : null,
                EmployeeDataId = x.ed != null ? x.ed.Id : (long?)null,
                UnitId = x.ed != null ? x.ed.UnitId : (long?)null
            })
            .ToListAsync();

        return new RepositoryBaseResponse<GetEmployeeListResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetEmployeeListResponse { Employee = employees }
        };
    }

    public async Task<RepositoryBaseResponse<GetEmployeeResponse>> GetEmployee(GetEmployeeRequest request)
    {
        var entity = await context.Employees
            .Where(e => e.Id == request.Id && e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse<GetEmployeeResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee not found" },
                Data = null!
            };
        }

        return new RepositoryBaseResponse<GetEmployeeResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetEmployeeResponse
            {
                Employee = new Employee
                {
                    Id = entity.Id,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Position = entity.Position,
                    CompanyId = entity.CompanyId
                }
            }
        };
    }

    public async Task<RepositoryBaseResponse> InsertEmployee(InsertEmployeeRequest request)
    {
        var company = await context.Companies.FindAsync(request.CompanyId);
        if (company == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company not found" }
            };
        }

        var entity = new EmployeeEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position ?? 0,
            CompanyId = request.CompanyId ?? 0,
            Company = company,
            CreatedAt = DateTime.UtcNow
        };

        context.Employees.Add(entity);
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee created successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> UpdateEmployee(long id, UpdateEmployeeRequest request)
    {
        var entity = await context.Employees
            .Where(e => e.Id == id && e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee not found" }
            };
        }

        if (request.FirstName != null) entity.FirstName = request.FirstName;
        if (request.LastName != null) entity.LastName = request.LastName;
        if (request.Position.HasValue) entity.Position = request.Position.Value;
        if (request.CompanyId.HasValue) entity.CompanyId = request.CompanyId.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee updated successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> DeleteEmployee(DeleteEmployeeRequest request)
    {
        var entity = await context.Employees
            .Where(e => e.Id == request.Id && e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee not found" }
            };
        }

        entity.DeletedAt = DateTime.UtcNow;

        var employeeData = await context.EmployeeData
            .Where(ed => ed.EmployeeId == request.Id && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (employeeData != null)
        {
            employeeData.DeletedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee deleted successfully" }
        };
    }

    public async Task<RepositoryBaseResponse<GetEmployeeDataResponse>> GetEmployeeData(GetEmployeeDataRequest request)
    {
        var entity = await context.EmployeeData
            .Where(ed => ed.Id == request.Id && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse<GetEmployeeDataResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee data not found" },
                Data = null!
            };
        }

        return new RepositoryBaseResponse<GetEmployeeDataResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetEmployeeDataResponse
            {
                Id = entity.Id,
                Email = entity.Email,
                TeamName = entity.TeamName,
                Role = entity.Role
            }
        };
    }

    public async Task<RepositoryBaseResponse> InsertEmployeeData(InsertEmployeeDataRequest request)
    {
        var employee = await context.Employees.FindAsync(request.EmployeeId);
        var businessUnit = await context.BusinessUnits.FindAsync(request.UnitId);

        if (employee == null || businessUnit == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee or business unit not found" }
            };
        }

        var entity = new EmployeeDataEntity
        {
            Email = request.Email ?? string.Empty,
            Password = request.Password ?? string.Empty,
            TeamName = request.TeamName ?? string.Empty,
            Role = request.Role ?? 0,
            EmployeeId = request.EmployeeId,
            Employee = employee,
            UnitId = request.UnitId,
            BusinessUnit = businessUnit,
            CreatedAt = DateTime.UtcNow
        };

        context.EmployeeData.Add(entity);
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee data created successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> UpdateEmployeeData(long id, UpdateEmployeeDataRequest request)
    {
        var entity = await context.EmployeeData
            .Where(ed => ed.Id == id && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee data not found" }
            };
        }

        if (request.Email != null) entity.Email = request.Email;
        if (request.Password != null) entity.Password = request.Password;
        if (request.TeamName != null) entity.TeamName = request.TeamName;
        if (request.Role.HasValue) entity.Role = request.Role.Value;
        if (request.UnitId.HasValue) entity.UnitId = request.UnitId.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee data updated successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> DeleteEmployeeData(DeleteEmployeeDataRequest request)
    {
        var entity = await context.EmployeeData
            .Where(ed => ed.Id == request.Id && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee data not found" }
            };
        }

        entity.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee data deleted successfully" }
        };
    }

    public async Task<RepositoryBaseResponse<CreateEmployeeFullData>> CreateEmployeeFull(CreateEmployeeFullRequest request)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var company = await context.Companies.FindAsync(request.CompanyId);
            var businessUnit = await context.BusinessUnits.FindAsync(request.UnitId);

            if (company == null || businessUnit == null)
            {
                return new RepositoryBaseResponse<CreateEmployeeFullData>
                {
                    Status = new RepositoryStatus { Code = "404", Message = "Company or business unit not found" },
                    Data = new CreateEmployeeFullData()
                };
            }

            var existingEmail = await context.EmployeeData
                .Where(ed => ed.Email == request.Email && ed.DeletedAt == null)
                .AnyAsync();

            if (existingEmail)
            {
                return new RepositoryBaseResponse<CreateEmployeeFullData>
                {
                    Status = new RepositoryStatus { Code = "409", Message = "Email already exists" },
                    Data = new CreateEmployeeFullData()
                };
            }

            var employee = new EmployeeEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Position = request.Position,
                CompanyId = request.CompanyId,
                Company = company,
                CreatedAt = DateTime.UtcNow
            };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var generatedPassword = Guid.NewGuid().ToString("N")[..12];

            var employeeData = new EmployeeDataEntity
            {
                Email = request.Email,
                Password = generatedPassword,
                TeamName = "Default",
                Role = request.Role,
                EmployeeId = employee.Id,
                Employee = employee,
                UnitId = request.UnitId,
                BusinessUnit = businessUnit,
                CreatedAt = DateTime.UtcNow
            };
            context.EmployeeData.Add(employeeData);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            await chatEventPublisher.PublishChatAccessGranted(request.UnitId, employee.Id);

            return new RepositoryBaseResponse<CreateEmployeeFullData>
            {
                Status = new RepositoryStatus { Code = "200", Message = "Employee created successfully" },
                Data = new CreateEmployeeFullData
                {
                    EmployeeId = employee.Id,
                    GeneratedPassword = generatedPassword
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new RepositoryBaseResponse<CreateEmployeeFullData>
            {
                Status = new RepositoryStatus
                {
                    Code = "500",
                    Message = "Employee creation failed",
                    Error = new RepositoryError
                    {
                        ErrorCode = "500",
                        ErrorMessage = ex.Message
                    }
                },
                Data = new CreateEmployeeFullData()
            };
        }
    }

    public async Task<RepositoryBaseResponse<CheckEmployeeUnitResponse>> CheckEmployeeUnit(CheckEmployeeUnitRequest request)
    {
        var employeeData = await context.EmployeeData
            .Include(ed => ed.BusinessUnit)
            .Where(ed => ed.EmployeeId == request.EmployeeId && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (employeeData == null)
        {
            return new RepositoryBaseResponse<CheckEmployeeUnitResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Employee data not found" },
                Data = new CheckEmployeeUnitResponse { UnitId = 0 }
            };
        }

        var unitId = employeeData.BusinessUnit?.DeletedAt == null ? employeeData.UnitId : 0;

        return new RepositoryBaseResponse<CheckEmployeeUnitResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new CheckEmployeeUnitResponse { UnitId = unitId }
        };
    }
}

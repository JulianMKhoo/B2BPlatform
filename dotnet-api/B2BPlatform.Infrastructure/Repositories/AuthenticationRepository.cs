using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Shared.Entities;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Repositories;

public class AuthenticationRepository(AppDbContext context, IChatEventPublisher chatEventPublisher) : IAuthenticationRepository
{
    private const string DefaultPassword = "password123";

    public async Task<RepositoryBaseResponse<CompanyOwnerLoginData>> CompanyOwnerLogin(CompanyOwnerLoginRequest request)
    {
        var owner = await context.CompanyOwners
            .Where(co => co.Id == request.Id && co.Password == request.Password && co.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (owner == null)
        {
            return new RepositoryBaseResponse<CompanyOwnerLoginData>
            {
                Status = new RepositoryStatus { Code = "401", Message = "Invalid credentials" },
                Data = new CompanyOwnerLoginData()
            };
        }

        return new RepositoryBaseResponse<CompanyOwnerLoginData>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Login successful" },
            Data = new CompanyOwnerLoginData { OwnerId = owner.Id, CompanyId = owner.CompanyId }
        };
    }

    public async Task<RepositoryBaseResponse<CompanyOwnerRegisterData>> CompanyOwnerRegister(CompanyOwnerRegisterRequest request)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var company = new CompanyEntity
            {
                CompanyName = request.CompanyName,
                CompanyAddress = request.CompanyAddress,
                ContactNumber = request.ContactNumber,
                ContractNumber = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            var employee = new EmployeeEntity
            {
                FirstName = "Owner",
                LastName = request.CompanyName,
                Position = 3, // Manager
                CompanyId = company.Id,
                Company = company,
                CreatedAt = DateTime.UtcNow
            };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var owner = new CompanyOwnerEntity
            {
                CompanyId = company.Id,
                Company = company,
                Password = DefaultPassword,
                CreatedAt = DateTime.UtcNow
            };
            context.CompanyOwners.Add(owner);
            await context.SaveChangesAsync();

            var businessUnit = new BusinessUnitEntity
            {
                BusinessUnitName = "Default",
                CompanyId = company.Id,
                Company = company,
                CompanyOwnerId = owner.Id,
                CompanyOwner = owner,
                CreatedAt = DateTime.UtcNow
            };
            context.BusinessUnits.Add(businessUnit);
            await context.SaveChangesAsync();

            var employeeData = new EmployeeDataEntity
            {
                Email = $"owner@{request.CompanyName.ToLower().Replace(" ", "")}.com",
                Password = DefaultPassword,
                TeamName = "Default",
                Role = 1,
                EmployeeId = employee.Id,
                Employee = employee,
                UnitId = businessUnit.Id,
                BusinessUnit = businessUnit,
                CreatedAt = DateTime.UtcNow
            };
            context.EmployeeData.Add(employeeData);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Publish event to Go Chat Service via Redis
            await chatEventPublisher.PublishWorkspaceCreated(
                businessUnit.Id,
                "General",
                owner.Id
            );

            return new RepositoryBaseResponse<CompanyOwnerRegisterData>
            {
                Status = new RepositoryStatus { Code = "200", Message = "Registration successful" },
                Data = new CompanyOwnerRegisterData { OwnerId = owner.Id, DefaultPassword = DefaultPassword }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new RepositoryBaseResponse<CompanyOwnerRegisterData>
            {
                Status = new RepositoryStatus
                {
                    Code = "500",
                    Message = "Registration failed",
                    Error = new RepositoryError
                    {
                        ErrorCode = "500",
                        ErrorMessage = ex.Message
                    }
                },
                Data = new CompanyOwnerRegisterData()
            };
        }
    }

    public async Task<RepositoryBaseResponse<EmployeeLoginData>> EmployeeLogin(EmployeeLoginRequest request)
    {
        var employeeData = await context.EmployeeData
            .Include(ed => ed.Employee)
            .Include(ed => ed.BusinessUnit)
            .Where(ed => ed.Email == request.Email && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (employeeData == null || employeeData.Password != request.Password)
        {
            return new RepositoryBaseResponse<EmployeeLoginData>
            {
                Status = new RepositoryStatus { Code = "401", Message = "Invalid credentials" },
                Data = new EmployeeLoginData()
            };
        }

        var unitId = employeeData.BusinessUnit?.DeletedAt == null ? employeeData.UnitId : 0;

        return new RepositoryBaseResponse<EmployeeLoginData>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Login successful" },
            Data = new EmployeeLoginData
            {
                EmployeeId = employeeData.EmployeeId,
                CompanyId = employeeData.Employee.CompanyId,
                UnitId = unitId,
                Role = employeeData.Role,
                FirstName = employeeData.Employee.FirstName,
                LastName = employeeData.Employee.LastName
            }
        };
    }

    public async Task<RepositoryBaseResponse> EmployeeRegister(EmployeeRegisterRequest request)
    {
        var existingEmail = await context.EmployeeData
            .Where(ed => ed.Email == request.Email && ed.DeletedAt == null)
            .AnyAsync();

        if (existingEmail)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "409", Message = "Email already exists" }
            };
        }

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Employee registration requires company context" }
        };
    }

    public async Task<RepositoryBaseResponse> ChangeOwnerPassword(ChangeOwnerPasswordRequest request)
    {
        var owner = await context.CompanyOwners
            .Where(co => co.Id == request.Id && co.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (owner == null || owner.Password != request.CurrentPassword)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "401", Message = "Invalid current password" }
            };
        }

        owner.Password = request.NewPassword;
        owner.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Password changed successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> ChangeEmployeePassword(ChangeEmployeePasswordRequest request)
    {
        var employeeData = await context.EmployeeData
            .Where(ed => ed.EmployeeId == request.EmployeeId && ed.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (employeeData == null || employeeData.Password != request.CurrentPassword)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "401", Message = "Invalid current password" }
            };
        }

        employeeData.Password = request.NewPassword;
        employeeData.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Password changed successfully" }
        };
    }
}


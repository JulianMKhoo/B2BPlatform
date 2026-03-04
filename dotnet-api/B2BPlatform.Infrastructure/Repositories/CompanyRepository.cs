using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Shared.Entities;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Repositories;

public class CompanyRepository(AppDbContext context) : ICompanyRepository
{
    public async Task<RepositoryBaseResponse<GetCompanyResponse>> GetCompany(GetCompanyRequest request)
    {
        var entity = await context.Companies
            .Where(c => c.Id == request.Id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse<GetCompanyResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company not found" },
                Data = null!
            };
        }

        return new RepositoryBaseResponse<GetCompanyResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetCompanyResponse
            {
                Id = entity.Id,
                CompanyName = entity.CompanyName,
                CompanyAddress = entity.CompanyAddress ?? string.Empty,
                ContractNumber = entity.ContractNumber.ToString()
            }
        };
    }

    public async Task<RepositoryBaseResponse> InsertCompany(InsertCompanyRequest request)
    {
        var entity = new CompanyEntity
        {
            CompanyName = request.CompanyName,
            CompanyAddress = request.CompanyAddress,
            ContractNumber = int.Parse(request.ContractNumber),
            CreatedAt = DateTime.UtcNow
        };

        context.Companies.Add(entity);
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Company created successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> UpdateCompany(long id, UpdateCompanyRequest request)
    {
        var entity = await context.Companies
            .Where(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company not found" }
            };
        }

        entity.CompanyName = request.CompanyName;
        entity.CompanyAddress = request.CompanyAddress;
        entity.ContractNumber = int.Parse(request.ContractNumber);
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Company updated successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> DeleteCompany(DeleteCompanyRequest request)
    {
        var entity = await context.Companies
            .Where(c => c.Id == request.Id && c.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company not found" }
            };
        }

        entity.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Company deleted successfully" }
        };
    }
}

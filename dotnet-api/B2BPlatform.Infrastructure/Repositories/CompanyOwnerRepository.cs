using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Repositories;

public class CompanyOwnerRepository(AppDbContext context) : ICompanyOwnerRepository
{
    public async Task<RepositoryBaseResponse<GetCompanyOwnerResponse>> GetCompanyOwner(GetCompanyOwnerRequest request)
    {
        var entity = await context.CompanyOwners
            .Where(co => co.Id == request.Id && co.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse<GetCompanyOwnerResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company owner not found" },
                Data = null!
            };
        }

        return new RepositoryBaseResponse<GetCompanyOwnerResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetCompanyOwnerResponse
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId
            }
        };
    }

    public async Task<RepositoryBaseResponse> UpdateCompanyOwner(long id, UpdateCompanyOwnerRequest request)
    {
        var entity = await context.CompanyOwners
            .Where(co => co.Id == id && co.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company owner not found" }
            };
        }

        entity.Password = request.Password;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Company owner updated successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> DeleteCompanyOwner(DeleteCompanyOwnerRequest request)
    {
        var entity = await context.CompanyOwners
            .Where(co => co.Id == request.Id && co.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company owner not found" }
            };
        }

        entity.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Company owner deleted successfully" }
        };
    }
}

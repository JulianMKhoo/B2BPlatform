using B2BPlatform.Infrastructure.Contexts;
using B2BPlatform.Shared.Entities;
using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Repositories;

public class BusinessUnitRepository(AppDbContext context) : IBusinessUnitRepository
{
    public async Task<RepositoryBaseResponse<GetBusinessUnitListResponse>> GetBusinessUnitList(GetBusinessUnitListRequest request)
    {
        var units = await context.BusinessUnits
            .Where(bu => bu.CompanyId == request.CompanyId && bu.DeletedAt == null)
            .Select(bu => new BusinessUnitItem
            {
                Id = bu.Id,
                BusinessUnitName = bu.BusinessUnitName
            })
            .ToListAsync();

        return new RepositoryBaseResponse<GetBusinessUnitListResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetBusinessUnitListResponse { BusinessUnits = units }
        };
    }

    public async Task<RepositoryBaseResponse<GetBusinessUnitResponse>> GetBusinessUnit(GetBusinessUnitRequest request)
    {
        var entity = await context.BusinessUnits
            .Where(bu => bu.Id == request.Id && bu.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse<GetBusinessUnitResponse>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Business unit not found" },
                Data = null!
            };
        }

        return new RepositoryBaseResponse<GetBusinessUnitResponse>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Success" },
            Data = new GetBusinessUnitResponse
            {
                Id = entity.Id,
                BusinessUnitName = entity.BusinessUnitName,
                CompanyId = entity.CompanyId,
                CompanyOwnerId = entity.CompanyOwnerId
            }
        };
    }

    public async Task<RepositoryBaseResponse<long>> InsertBusinessUnit(InsertBusinessUnitRequest request)
    {
        var company = await context.Companies.FindAsync(request.CompanyId);
        var owner = await context.CompanyOwners.FindAsync(request.CompanyOwnerId);

        if (company == null || owner == null)
        {
            return new RepositoryBaseResponse<long>
            {
                Status = new RepositoryStatus { Code = "404", Message = "Company or owner not found" },
                Data = 0
            };
        }

        var entity = new BusinessUnitEntity
        {
            BusinessUnitName = request.BusinessUnitName,
            CompanyId = request.CompanyId,
            Company = company,
            CompanyOwnerId = request.CompanyOwnerId,
            CompanyOwner = owner,
            CreatedAt = DateTime.UtcNow
        };

        context.BusinessUnits.Add(entity);
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse<long>
        {
            Status = new RepositoryStatus { Code = "200", Message = "Business unit created successfully" },
            Data = entity.Id
        };
    }

    public async Task<RepositoryBaseResponse> UpdateBusinessUnit(long id, UpdateBusinessUnitRequest request)
    {
        var entity = await context.BusinessUnits
            .Where(bu => bu.Id == id && bu.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Business unit not found" }
            };
        }

        if (request.BusinessUnitName != null) entity.BusinessUnitName = request.BusinessUnitName;
        entity.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Business unit updated successfully" }
        };
    }

    public async Task<RepositoryBaseResponse> DeleteBusinessUnit(DeleteBusinessUnitRequest request)
    {
        var entity = await context.BusinessUnits
            .Where(bu => bu.Id == request.Id && bu.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return new RepositoryBaseResponse
            {
                Status = new RepositoryStatus { Code = "404", Message = "Business unit not found" }
            };
        }

        entity.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return new RepositoryBaseResponse
        {
            Status = new RepositoryStatus { Code = "200", Message = "Business unit deleted successfully" }
        };
    }
}

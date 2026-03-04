using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Repositories;

public interface IBusinessUnitRepository
{
    Task<RepositoryBaseResponse<GetBusinessUnitListResponse>> GetBusinessUnitList(GetBusinessUnitListRequest request);
    Task<RepositoryBaseResponse<GetBusinessUnitResponse>> GetBusinessUnit(GetBusinessUnitRequest request);
    Task<RepositoryBaseResponse<long>> InsertBusinessUnit(InsertBusinessUnitRequest request);
    Task<RepositoryBaseResponse> UpdateBusinessUnit(long id, UpdateBusinessUnitRequest request);
    Task<RepositoryBaseResponse> DeleteBusinessUnit(DeleteBusinessUnitRequest request);
}

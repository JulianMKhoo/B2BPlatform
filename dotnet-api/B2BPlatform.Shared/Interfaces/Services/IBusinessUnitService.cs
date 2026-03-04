using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface IBusinessUnitService
{
    public Task<GetBusinessUnitListResponse> GetBusinessUnitList(GetBusinessUnitListRequest request);
    public Task<GetBusinessUnitResponse> GetBusinessUnit(GetBusinessUnitRequest request);
    public Task<InsertBusinessUnitResponse> InsertBusinessUnit(InsertBusinessUnitRequest request);
    public Task<UpdateBusinessUnitResponse> UpdateBusinessUnit(UpdateBusinessUnitRequest request);
    public Task<DeleteBusinessUnitResponse> DeleteBusinessUnit(DeleteBusinessUnitRequest request);
}
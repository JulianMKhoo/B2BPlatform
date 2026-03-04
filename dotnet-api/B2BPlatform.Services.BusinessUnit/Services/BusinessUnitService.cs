using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Services.BusinessUnit.Services;

public class BusinessUnitService(
    IBusinessUnitRepository businessUnitRepository,
    IChatEventPublisher chatEventPublisher) : IBusinessUnitService
{
    public async Task<GetBusinessUnitListResponse> GetBusinessUnitList(GetBusinessUnitListRequest request)
    {
        var result = await businessUnitRepository.GetBusinessUnitList(request);
        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<GetBusinessUnitResponse> GetBusinessUnit(GetBusinessUnitRequest request)
    {
        var result = await businessUnitRepository.GetBusinessUnit(request);
        if (result.Data == null)
        {
            return new GetBusinessUnitResponse
            {
                Id = 0,
                BusinessUnitName = string.Empty,
                CompanyId = 0,
                CompanyOwnerId = 0,
                Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
            };
        }

        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<InsertBusinessUnitResponse> InsertBusinessUnit(InsertBusinessUnitRequest request)
    {
        var result = await businessUnitRepository.InsertBusinessUnit(request);
        if (result.Status.Code == "200")
        {
            // Publish event to Go Chat Service via Redis to auto-provision workspace
            await chatEventPublisher.PublishWorkspaceCreated(
                result.Data, // newly created BU ID
                $"{request.BusinessUnitName} Chat",
                request.CompanyOwnerId
            );
        }

        return new InsertBusinessUnitResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<UpdateBusinessUnitResponse> UpdateBusinessUnit(UpdateBusinessUnitRequest request)
    {
        var result = await businessUnitRepository.UpdateBusinessUnit(request.Id, request);
        return new UpdateBusinessUnitResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<DeleteBusinessUnitResponse> DeleteBusinessUnit(DeleteBusinessUnitRequest request)
    {
        var result = await businessUnitRepository.DeleteBusinessUnit(request);
        if (result.Status.Code == "200")
        {
            // Publish event to Go Chat Service to delete workspace
            await chatEventPublisher.PublishWorkspaceDeleted(request.Id);
        }

        return new DeleteBusinessUnitResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }
}

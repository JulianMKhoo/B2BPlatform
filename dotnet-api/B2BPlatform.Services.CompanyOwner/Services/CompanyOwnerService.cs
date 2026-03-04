using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Services.CompanyOwner.Services;

public class CompanyOwnerService(ICompanyOwnerRepository companyOwnerRepository) : ICompanyOwnerService
{
    public async Task<GetCompanyOwnerResponse> GetCompanyOwner(GetCompanyOwnerRequest request)
    {
        var result = await companyOwnerRepository.GetCompanyOwner(request);
        if (result.Data == null)
        {
            return new GetCompanyOwnerResponse
            {
                Id = 0,
                CompanyId = 0,
                Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
            };
        }

        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<UpdateCompanyOwnerResponse> UpdateCompanyOwner(UpdateCompanyOwnerRequest request)
    {
        var result = await companyOwnerRepository.UpdateCompanyOwner(request.Id, request);
        return new UpdateCompanyOwnerResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<DeleteCompanyOwnerResponse> DeleteCompanyOwner(DeleteCompanyOwnerRequest request)
    {
        var result = await companyOwnerRepository.DeleteCompanyOwner(request);
        return new DeleteCompanyOwnerResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }
}

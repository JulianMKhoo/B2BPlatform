using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface ICompanyOwnerService
{
    public Task<GetCompanyOwnerResponse> GetCompanyOwner(GetCompanyOwnerRequest request);
    public Task<UpdateCompanyOwnerResponse> UpdateCompanyOwner(UpdateCompanyOwnerRequest request);
    public Task<DeleteCompanyOwnerResponse> DeleteCompanyOwner(DeleteCompanyOwnerRequest request);
}
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Repositories;

public interface ICompanyOwnerRepository
{
    Task<RepositoryBaseResponse<GetCompanyOwnerResponse>> GetCompanyOwner(GetCompanyOwnerRequest request);
    Task<RepositoryBaseResponse> UpdateCompanyOwner(long id, UpdateCompanyOwnerRequest request);
    Task<RepositoryBaseResponse> DeleteCompanyOwner(DeleteCompanyOwnerRequest request);
}

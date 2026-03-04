using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<RepositoryBaseResponse<GetCompanyResponse>> GetCompany(GetCompanyRequest request);
    Task<RepositoryBaseResponse> InsertCompany(InsertCompanyRequest request);
    Task<RepositoryBaseResponse> UpdateCompany(long id, UpdateCompanyRequest request);
    Task<RepositoryBaseResponse> DeleteCompany(DeleteCompanyRequest request);
}

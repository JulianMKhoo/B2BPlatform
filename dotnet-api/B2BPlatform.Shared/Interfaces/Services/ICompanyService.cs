using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface ICompanyService
{
    public Task<GetCompanyResponse> GetCompany(GetCompanyRequest request);
    public Task<InsertCompanyResponse> InsertCompany(InsertCompanyRequest request);
    public Task<UpdateCompanyResponse> UpdateCompany(UpdateCompanyRequest request);
    public Task<DeleteCompanyResponse> DeleteCompany(DeleteCompanyRequest request);
}

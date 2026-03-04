using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Service.Dashboard.Services;

public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
{
    public async Task<GetCompanyResponse> GetCompany(GetCompanyRequest request)
    {
        var result = await companyRepository.GetCompany(request);
        if (result.Data == null)
        {
            return new GetCompanyResponse
            {
                Id = 0,
                CompanyName = string.Empty,
                CompanyAddress = string.Empty,
                ContractNumber = string.Empty,
                Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
            };
        }

        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<InsertCompanyResponse> InsertCompany(InsertCompanyRequest request)
    {
        var result = await companyRepository.InsertCompany(request);
        return new InsertCompanyResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<UpdateCompanyResponse> UpdateCompany(UpdateCompanyRequest request)
    {
        var result = await companyRepository.UpdateCompany(request.Id, request);
        return new UpdateCompanyResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<DeleteCompanyResponse> DeleteCompany(DeleteCompanyRequest request)
    {
        var result = await companyRepository.DeleteCompany(request);
        return new DeleteCompanyResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }
}

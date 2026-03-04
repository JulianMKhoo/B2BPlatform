using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Repositories;

public interface IAuthenticationRepository
{
    public Task<RepositoryBaseResponse<CompanyOwnerLoginData>> CompanyOwnerLogin(CompanyOwnerLoginRequest request);
    public Task<RepositoryBaseResponse<CompanyOwnerRegisterData>> CompanyOwnerRegister(CompanyOwnerRegisterRequest request);
    public Task<RepositoryBaseResponse<EmployeeLoginData>> EmployeeLogin(EmployeeLoginRequest request);
    public Task<RepositoryBaseResponse> EmployeeRegister(EmployeeRegisterRequest request);
    public Task<RepositoryBaseResponse> ChangeOwnerPassword(ChangeOwnerPasswordRequest request);
    public Task<RepositoryBaseResponse> ChangeEmployeePassword(ChangeEmployeePasswordRequest request);
}

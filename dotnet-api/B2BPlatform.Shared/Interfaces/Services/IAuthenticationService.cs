using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface IAuthenticationService
{
    public Task<CompanyOwnerLoginResponse> CompanyOwnerLogin(CompanyOwnerLoginRequest request);
    public Task<CompanyOwnerRegisterResponse> CompanyOwnerRegister(CompanyOwnerRegisterRequest request);
    public Task<EmployeeLoginResponse> EmployeeLogin(EmployeeLoginRequest request);
    public Task<EmployeeRegisterResponse> EmployeeRegister(EmployeeRegisterRequest request);
    public Task<ChangeOwnerPasswordResponse> ChangeOwnerPassword(ChangeOwnerPasswordRequest request);
    public Task<ChangeEmployeePasswordResponse> ChangeEmployeePassword(ChangeEmployeePasswordRequest request);
}
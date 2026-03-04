using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Service.Authentication.Services;

public class AuthenticationService(IAuthenticationRepository authenticationRepository) : IAuthenticationService
{
    public async Task<CompanyOwnerLoginResponse> CompanyOwnerLogin(CompanyOwnerLoginRequest request)
    {
        var result = await authenticationRepository.CompanyOwnerLogin(request);
        return new CompanyOwnerLoginResponse
        {
            Id = result.Data.OwnerId,
            CompanyId = result.Data.CompanyId,
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }

    public async Task<CompanyOwnerRegisterResponse> CompanyOwnerRegister(CompanyOwnerRegisterRequest request)
    {
        var result = await authenticationRepository.CompanyOwnerRegister(request);
        return new CompanyOwnerRegisterResponse
        {
            OwnerId = result.Data.OwnerId,
            DefaultPassword = result.Data.DefaultPassword,
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }

    public async Task<EmployeeLoginResponse> EmployeeLogin(EmployeeLoginRequest request)
    {
        var result = await authenticationRepository.EmployeeLogin(request);
        return new EmployeeLoginResponse
        {
            Id = result.Data.EmployeeId,
            CompanyId = result.Data.CompanyId,
            UnitId = result.Data.UnitId,
            Role = result.Data.Role,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName,
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }

    public async Task<EmployeeRegisterResponse> EmployeeRegister(EmployeeRegisterRequest request)
    {
        var result = await authenticationRepository.EmployeeRegister(request);
        return new EmployeeRegisterResponse
        {
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }

    public async Task<ChangeOwnerPasswordResponse> ChangeOwnerPassword(ChangeOwnerPasswordRequest request)
    {
        var result = await authenticationRepository.ChangeOwnerPassword(request);
        return new ChangeOwnerPasswordResponse
        {
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }

    public async Task<ChangeEmployeePasswordResponse> ChangeEmployeePassword(ChangeEmployeePasswordRequest request)
    {
        var result = await authenticationRepository.ChangeEmployeePassword(request);
        return new ChangeEmployeePasswordResponse
        {
            Status = new ServiceStatus
            {
                Code = result.Status.Code,
                Message = result.Status.Message
            }
        };
    }
}

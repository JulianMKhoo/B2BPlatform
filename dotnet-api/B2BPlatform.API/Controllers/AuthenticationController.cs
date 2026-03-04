using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace B2BPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class AuthenticationController(
    IAuthenticationService authenticationService)
    : ControllerBase
{
    [HttpPost("company_owner/auth/register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyOwnerRegisterResponse))]
    public async Task<IActionResult> CompanyOwnerRegister([FromBody] CompanyOwnerRegisterRequest request)
    {
        var response = await authenticationService.CompanyOwnerRegister(request);
        return Ok(response);
    }

    [HttpPost("company_owner/auth/login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyOwnerLoginResponse))]
    public async Task<IActionResult> CompanyOwnerLogin([FromBody] CompanyOwnerLoginRequest request)
    {
        var response = await authenticationService.CompanyOwnerLogin(request);
        return Ok(response);
    }

    [HttpPost("employee/auth/login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeLoginResponse))]
    public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeLoginRequest request)
    {
        var response = await authenticationService.EmployeeLogin(request);
        return Ok(response);
    }

    [HttpPost("employee/auth/register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeRegisterResponse))]
    public async Task<IActionResult> EmployeeRegister([FromBody] EmployeeRegisterRequest request)
    {
        var response = await authenticationService.EmployeeRegister(request);
        return Ok(response);
    }

    [HttpPost("company_owner/auth/change_password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChangeOwnerPasswordResponse))]
    public async Task<IActionResult> ChangeOwnerPassword([FromBody] ChangeOwnerPasswordRequest request)
    {
        var response = await authenticationService.ChangeOwnerPassword(request);
        return Ok(response);
    }

    [HttpPost("employee/auth/change_password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChangeEmployeePasswordResponse))]
    public async Task<IActionResult> ChangeEmployeePassword([FromBody] ChangeEmployeePasswordRequest request)
    {
        var response = await authenticationService.ChangeEmployeePassword(request);
        return Ok(response);
    }
}

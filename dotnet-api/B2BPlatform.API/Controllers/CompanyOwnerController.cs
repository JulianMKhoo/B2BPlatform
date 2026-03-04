using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace B2BPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class CompanyOwnerController(ICompanyOwnerService companyOwnerService) : ControllerBase
{
    [HttpPost("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCompanyOwnerResponse))]
    public async Task<IActionResult> GetCompanyOwner([FromBody] GetCompanyOwnerRequest request)
    {
        var response = await companyOwnerService.GetCompanyOwner(request);
        return Ok(response);
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCompanyOwnerResponse))]
    public async Task<IActionResult> UpdateCompanyOwner([FromBody] UpdateCompanyOwnerRequest request)
    {
        var response = await companyOwnerService.UpdateCompanyOwner(request);
        return Ok(response);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteCompanyOwnerResponse))]
    public async Task<IActionResult> DeleteCompanyOwner([FromBody] DeleteCompanyOwnerRequest request)
    {
        var response = await companyOwnerService.DeleteCompanyOwner(request);
        return Ok(response);
    }
}

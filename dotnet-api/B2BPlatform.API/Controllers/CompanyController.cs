using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace B2BPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpPost("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCompanyResponse))]
    public async Task<IActionResult> GetCompany([FromBody] GetCompanyRequest request)
    {
        var response = await companyService.GetCompany(request);
        return Ok(response);
    }

    [HttpPost("insert")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InsertCompanyResponse))]
    public async Task<IActionResult> InsertCompany([FromBody] InsertCompanyRequest request)
    {
        var response = await companyService.InsertCompany(request);
        return Ok(response);
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCompanyResponse))]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request)
    {
        var response = await companyService.UpdateCompany(request);
        return Ok(response);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteCompanyResponse))]
    public async Task<IActionResult> DeleteCompany([FromBody] DeleteCompanyRequest request)
    {
        var response = await companyService.DeleteCompany(request);
        return Ok(response);
    }
}

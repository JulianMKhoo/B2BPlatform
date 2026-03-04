using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace B2BPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class BusinessUnitController(IBusinessUnitService businessUnitService) : ControllerBase
{
    [HttpPost("get_all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBusinessUnitListResponse))]
    public async Task<IActionResult> GetBusinessUnitList([FromBody] GetBusinessUnitListRequest request)
    {
        var response = await businessUnitService.GetBusinessUnitList(request);
        return Ok(response);
    }

    [HttpPost("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBusinessUnitResponse))]
    public async Task<IActionResult> GetBusinessUnit([FromBody] GetBusinessUnitRequest request)
    {
        var response = await businessUnitService.GetBusinessUnit(request);
        return Ok(response);
    }

    [HttpPost("insert")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InsertBusinessUnitResponse))]
    public async Task<IActionResult> InsertBusinessUnit([FromBody] InsertBusinessUnitRequest request)
    {
        var response = await businessUnitService.InsertBusinessUnit(request);
        return Ok(response);
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateBusinessUnitResponse))]
    public async Task<IActionResult> UpdateBusinessUnit([FromBody] UpdateBusinessUnitRequest request)
    {
        var response = await businessUnitService.UpdateBusinessUnit(request);
        return Ok(response);
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteBusinessUnitResponse))]
    public async Task<IActionResult> DeleteBusinessUnit([FromBody] DeleteBusinessUnitRequest request)
    {
        var response = await businessUnitService.DeleteBusinessUnit(request);
        return Ok(response);
    }
}

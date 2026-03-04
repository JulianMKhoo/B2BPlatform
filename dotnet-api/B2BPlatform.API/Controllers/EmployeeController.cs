using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models;
using B2BPlatform.Shared.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace B2BPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]/v1")]
public class EmployeeController(IEmployeeService employeeService): ControllerBase
{
    [HttpPost("get_all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeListResponse))]
    public async Task<IActionResult> GetEmployeeList([FromBody] GetEmployeeListRequest request)
    {
        var response = await employeeService.GetEmployeeList(request);
        return Ok(response);   
    }
    
    [HttpPost("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeResponse))]
    public async Task<IActionResult> GetEmployee([FromBody] GetEmployeeRequest request)
    {
        var response = await employeeService.GetEmployee(request);
        return Ok(response);   
    }
    
    [HttpPost("insert")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InsertEmployeeResponse))]
    public async Task<IActionResult> InsertEmployee([FromBody] InsertEmployeeRequest request)
    {
        var response = await employeeService.InsertEmployee(request);
        return Ok(response);   
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeResponse))]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request)
    {
        var response = await  employeeService.UpdateEmployee(request);
        return Ok(response);   
    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteEmployeeResponse))]
    public async Task<IActionResult> DeleteEmployee([FromBody] DeleteEmployeeRequest request)
    {
        var response = await employeeService.DeleteEmployee(request);
        return Ok(response);   
    }
    
    [HttpPost("data/get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeDataResponse))]
    public async Task<IActionResult> GetEmployeeData([FromBody] GetEmployeeDataRequest request)
    {
        var response = await employeeService.GetEmployeeData(request);
        return Ok(response);   
    }
    
    [HttpPost("data/insert")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InsertEmployeeDataResponse))]
    public async Task<IActionResult> InsertEmployeeData([FromBody] InsertEmployeeDataRequest request)
    {
        var response = await employeeService.InsertEmployeeData(request);
        return Ok(response);   
    }
    
    [HttpPost("data/update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeDataResponse))]
    public async Task<IActionResult> UpdateEmployeeData([FromBody] UpdateEmployeeDataRequest request)
    {
        var response = await employeeService.UpdateEmployeeData(request);
        return Ok(response);   
    }
    
    [HttpPost("data/delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteEmployeeDataResponse))]
    public async Task<IActionResult> DeleteEmployeeData([FromBody] DeleteEmployeeDataRequest request)
    {
        var response = await employeeService.DeleteEmployeeData(request);
        return Ok(response);
    }

    [HttpPost("create_full")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateEmployeeFullResponse))]
    public async Task<IActionResult> CreateEmployeeFull([FromBody] CreateEmployeeFullRequest request)
    {
        var response = await employeeService.CreateEmployeeFull(request);
        return Ok(response);
    }

    [HttpPost("check_unit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckEmployeeUnitResponse))]
    public async Task<IActionResult> CheckEmployeeUnit([FromBody] CheckEmployeeUnitRequest request)
    {
        var response = await employeeService.CheckEmployeeUnit(request);
        return Ok(response);
    }
}
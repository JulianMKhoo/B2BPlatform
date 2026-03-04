using B2BPlatform.Shared.Interfaces.Repositories;
using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Service.Employees.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public async Task<GetEmployeeListResponse> GetEmployeeList(GetEmployeeListRequest request)
    {
        var result = await employeeRepository.GetEmployeeList(request);
        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<GetEmployeeResponse> GetEmployee(GetEmployeeRequest request)
    {
        var result = await employeeRepository.GetEmployee(request);
        if (result.Data == null)
        {
            return new GetEmployeeResponse
            {
                Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
            };
        }

        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<InsertEmployeeResponse> InsertEmployee(InsertEmployeeRequest request)
    {
        var result = await employeeRepository.InsertEmployee(request);
        return new InsertEmployeeResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request)
    {
        var result = await employeeRepository.UpdateEmployee(request.Id, request);
        return new UpdateEmployeeResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<DeleteEmployeeResponse> DeleteEmployee(DeleteEmployeeRequest request)
    {
        var result = await employeeRepository.DeleteEmployee(request);
        return new DeleteEmployeeResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<GetEmployeeDataResponse> GetEmployeeData(GetEmployeeDataRequest request)
    {
        var result = await employeeRepository.GetEmployeeData(request);
        if (result.Data == null)
        {
            return new GetEmployeeDataResponse
            {
                Id = 0,
                Email = string.Empty,
                Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
            };
        }

        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }

    public async Task<InsertEmployeeDataResponse> InsertEmployeeData(InsertEmployeeDataRequest request)
    {
        var result = await employeeRepository.InsertEmployeeData(request);
        return new InsertEmployeeDataResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<UpdateEmployeeDataResponse> UpdateEmployeeData(UpdateEmployeeDataRequest request)
    {
        var result = await employeeRepository.UpdateEmployeeData(request.Id, request);
        return new UpdateEmployeeDataResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<DeleteEmployeeDataResponse> DeleteEmployeeData(DeleteEmployeeDataRequest request)
    {
        var result = await employeeRepository.DeleteEmployeeData(request);
        return new DeleteEmployeeDataResponse
        {
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<CreateEmployeeFullResponse> CreateEmployeeFull(CreateEmployeeFullRequest request)
    {
        var result = await employeeRepository.CreateEmployeeFull(request);
        return new CreateEmployeeFullResponse
        {
            EmployeeId = result.Data.EmployeeId,
            GeneratedPassword = result.Data.GeneratedPassword,
            Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message }
        };
    }

    public async Task<CheckEmployeeUnitResponse> CheckEmployeeUnit(CheckEmployeeUnitRequest request)
    {
        var result = await employeeRepository.CheckEmployeeUnit(request);
        result.Data.Status = new ServiceStatus { Code = result.Status.Code, Message = result.Status.Message };
        return result.Data;
    }
}

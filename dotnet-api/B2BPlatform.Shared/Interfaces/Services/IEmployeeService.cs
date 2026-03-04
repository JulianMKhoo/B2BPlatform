using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface IEmployeeService
{
    public Task<GetEmployeeListResponse> GetEmployeeList(GetEmployeeListRequest request);
    public Task<GetEmployeeResponse> GetEmployee(GetEmployeeRequest request);
    public Task<InsertEmployeeResponse> InsertEmployee(InsertEmployeeRequest request);
    public Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request);
    public Task<DeleteEmployeeResponse> DeleteEmployee(DeleteEmployeeRequest request);
    public Task<GetEmployeeDataResponse> GetEmployeeData(GetEmployeeDataRequest request);
    public Task<InsertEmployeeDataResponse> InsertEmployeeData(InsertEmployeeDataRequest request);
    public Task<UpdateEmployeeDataResponse> UpdateEmployeeData(UpdateEmployeeDataRequest request);
    public Task<DeleteEmployeeDataResponse> DeleteEmployeeData(DeleteEmployeeDataRequest request);
    public Task<CreateEmployeeFullResponse> CreateEmployeeFull(CreateEmployeeFullRequest request);
    public Task<CheckEmployeeUnitResponse> CheckEmployeeUnit(CheckEmployeeUnitRequest request);
}
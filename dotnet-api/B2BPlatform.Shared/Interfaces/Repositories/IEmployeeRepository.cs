using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<RepositoryBaseResponse<GetEmployeeListResponse>> GetEmployeeList(GetEmployeeListRequest request);
    Task<RepositoryBaseResponse<GetEmployeeResponse>> GetEmployee(GetEmployeeRequest request);
    Task<RepositoryBaseResponse> InsertEmployee(InsertEmployeeRequest request);
    Task<RepositoryBaseResponse> UpdateEmployee(long id, UpdateEmployeeRequest request);
    Task<RepositoryBaseResponse> DeleteEmployee(DeleteEmployeeRequest request);
    Task<RepositoryBaseResponse<GetEmployeeDataResponse>> GetEmployeeData(GetEmployeeDataRequest request);
    Task<RepositoryBaseResponse> InsertEmployeeData(InsertEmployeeDataRequest request);
    Task<RepositoryBaseResponse> UpdateEmployeeData(long id, UpdateEmployeeDataRequest request);
    Task<RepositoryBaseResponse> DeleteEmployeeData(DeleteEmployeeDataRequest request);
    Task<RepositoryBaseResponse<CreateEmployeeFullData>> CreateEmployeeFull(CreateEmployeeFullRequest request);
    Task<RepositoryBaseResponse<CheckEmployeeUnitResponse>> CheckEmployeeUnit(CheckEmployeeUnitRequest request);
}

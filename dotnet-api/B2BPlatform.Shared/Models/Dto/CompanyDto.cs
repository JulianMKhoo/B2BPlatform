using B2BPlatform.Shared.Models.Commons;

namespace B2BPlatform.Shared.Models.Dto;

public record GetCompanyRequest
{
    public long Id { get; set; }
}

public record GetCompanyResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyAddress { get; set; }
    public required string ContractNumber { get; set; }
}

public record InsertCompanyRequest
{
    public required string CompanyName { get; set; }
    public required string CompanyAddress { get; set; }
    public required string ContractNumber { get; set; }
}
public record InsertCompanyResponse: ServiceBaseResponse;

public record UpdateCompanyRequest
{
    public long Id { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyAddress { get; set; }
    public required string ContractNumber { get; set; }
}
public record UpdateCompanyResponse: ServiceBaseResponse;

public record DeleteCompanyRequest
{
    public long Id { get; set; }
}
public record DeleteCompanyResponse: ServiceBaseResponse;

public record GetCompanyOwnerRequest
{
    public long Id { get; set; }
}

public record GetCompanyOwnerResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
}

public record UpdateCompanyOwnerRequest
{
    public long Id { get; set; }
    public required string Password { get; set; }
}
public record UpdateCompanyOwnerResponse: ServiceBaseResponse;

public record DeleteCompanyOwnerRequest
{
    public long Id { get; set; }
}
public record DeleteCompanyOwnerResponse: ServiceBaseResponse;
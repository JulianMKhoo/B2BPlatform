using B2BPlatform.Shared.Models.Commons;

namespace B2BPlatform.Shared.Models.Dto;

public record GetBusinessUnitListRequest
{
    public long CompanyId { get; set; }
}

public record GetBusinessUnitListResponse : ServiceBaseResponse
{
    public List<BusinessUnitItem>? BusinessUnits { get; set; }
}

public record BusinessUnitItem
{
    public long Id { get; set; }
    public required string BusinessUnitName { get; set; }
}

public record GetBusinessUnitRequest
{
    public long Id { get; set; }
}

public record GetBusinessUnitResponse : ServiceBaseResponse
{
    public long Id { get; set; }
    public required string BusinessUnitName { get; set; }
    public required long CompanyId { get; set; }
    public required long CompanyOwnerId { get; set; }
}

public record InsertBusinessUnitRequest
{
    public required string BusinessUnitName { get; set; }
    public required long CompanyId { get; set; }
    public required long CompanyOwnerId { get; set; }
}

public record InsertBusinessUnitResponse: ServiceBaseResponse;

public record UpdateBusinessUnitRequest
{
    public long Id { get; set; }
    public string? BusinessUnitName { get; set; }
}
public record UpdateBusinessUnitResponse: ServiceBaseResponse;

public record DeleteBusinessUnitRequest
{
    public long Id { get; set; }
}
public record DeleteBusinessUnitResponse: ServiceBaseResponse;
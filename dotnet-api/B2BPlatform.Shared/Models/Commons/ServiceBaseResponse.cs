namespace B2BPlatform.Shared.Models.Commons;

public record ServiceBaseResponse
{
    public ServiceStatus? Status { get; set; }
}

public record ServiceStatus
{
    public required string Code { get; set; }
    public required string Message { get; set; }
    public ServiceError? Error { get; set; }
}

public record ServiceError
{
    public required string ErrorCode { get; set; }
    public required string ErrorMessage { get; set; }
}
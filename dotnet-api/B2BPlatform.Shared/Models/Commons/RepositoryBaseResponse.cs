namespace B2BPlatform.Shared.Models.Commons;

public class RepositoryBaseResponse<T>
{
    public required RepositoryStatus Status { get; set; }
    public required T Data { get; set; }
}

public class RepositoryBaseResponse
{
    public required RepositoryStatus Status { get; set; }
}

public record RepositoryStatus
{
    public required string Code { get; set; }
    public required string Message { get; set; }
    public RepositoryError? Error { get; set; }
}

public record RepositoryError
{
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}

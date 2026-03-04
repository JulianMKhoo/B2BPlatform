using B2BPlatform.Shared.Models.Commons;

namespace B2BPlatform.Shared.Models.Dto;

public record GrantChatAccessRequest
{
    public long UnitId { get; set; }
    public long EmployeeId { get; set; }
}
public record GrantChatAccessResponse : ServiceBaseResponse;

public record RevokeChatAccessRequest
{
    public long UnitId { get; set; }
    public long EmployeeId { get; set; }
}
public record RevokeChatAccessResponse : ServiceBaseResponse;

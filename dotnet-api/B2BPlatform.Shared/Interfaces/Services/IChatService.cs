using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Shared.Interfaces.Services;

public interface IChatService
{
    public Task<GrantChatAccessResponse> GrantChatAccess(GrantChatAccessRequest request);
    public Task<RevokeChatAccessResponse> RevokeChatAccess(RevokeChatAccessRequest request);
}

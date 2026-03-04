using B2BPlatform.Shared.Interfaces.Services;
using B2BPlatform.Shared.Models.Commons;
using B2BPlatform.Shared.Models.Dto;

namespace B2BPlatform.Services.Chats.Services;

public class ChatService(IChatEventPublisher chatEventPublisher) : IChatService
{
    public async Task<GrantChatAccessResponse> GrantChatAccess(GrantChatAccessRequest request)
    {
        await chatEventPublisher.PublishChatAccessGranted(request.UnitId, request.EmployeeId);
        return new GrantChatAccessResponse
        {
            Status = new ServiceStatus { Code = "200", Message = "Chat access grant event published" }
        };
    }

    public async Task<RevokeChatAccessResponse> RevokeChatAccess(RevokeChatAccessRequest request)
    {
        await chatEventPublisher.PublishChatAccessRevoked(request.UnitId, request.EmployeeId);
        return new RevokeChatAccessResponse
        {
            Status = new ServiceStatus { Code = "200", Message = "Chat access revoke event published" }
        };
    }
}

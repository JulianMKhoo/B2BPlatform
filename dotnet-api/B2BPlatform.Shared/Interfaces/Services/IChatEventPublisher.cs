namespace B2BPlatform.Shared.Interfaces.Services;

public interface IChatEventPublisher
{
    Task PublishWorkspaceCreated(long unitId, string workspaceName, long companyOwnerId);
    Task PublishWorkspaceDeleted(long unitId);
    Task PublishChatAccessGranted(long unitId, long employeeId);
    Task PublishChatAccessRevoked(long unitId, long employeeId);
}

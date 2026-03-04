using System.Text.Json;
using B2BPlatform.Shared.Interfaces.Services;
using StackExchange.Redis;

namespace B2BPlatform.Infrastructure.Services;

public class RedisChatEventPublisher(IConnectionMultiplexer redis) : IChatEventPublisher
{
    private const string Channel = "chat_events";

    public async Task PublishWorkspaceCreated(long unitId, string workspaceName, long companyOwnerId)
    {
        var message = JsonSerializer.Serialize(new
        {
            type = "workspace_created",
            unit_id = unitId,
            workspace_name = workspaceName,
            admin_id = companyOwnerId
        });

        var subscriber = redis.GetSubscriber();
        await subscriber.PublishAsync(RedisChannel.Literal(Channel), message);
    }

    public async Task PublishWorkspaceDeleted(long unitId)
    {
        var message = JsonSerializer.Serialize(new
        {
            type = "workspace_deleted",
            unit_id = unitId
        });

        var subscriber = redis.GetSubscriber();
        await subscriber.PublishAsync(RedisChannel.Literal(Channel), message);
    }

    public async Task PublishChatAccessGranted(long unitId, long employeeId)
    {
        var message = JsonSerializer.Serialize(new
        {
            type = "chat_access_granted",
            unit_id = unitId,
            employee_id = employeeId
        });

        var subscriber = redis.GetSubscriber();
        await subscriber.PublishAsync(RedisChannel.Literal(Channel), message);
    }

    public async Task PublishChatAccessRevoked(long unitId, long employeeId)
    {
        var message = JsonSerializer.Serialize(new
        {
            type = "chat_access_revoked",
            unit_id = unitId,
            employee_id = employeeId
        });

        var subscriber = redis.GetSubscriber();
        await subscriber.PublishAsync(RedisChannel.Literal(Channel), message);
    }
}

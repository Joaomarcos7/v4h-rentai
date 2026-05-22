using Microsoft.AspNetCore.SignalR;
using V4H.Application.Common.Interfaces;
using V4H.Infrastructure.Hubs;

namespace V4H.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotificationService(IHubContext<NotificationHub> hub) => _hub = hub;

    public Task SendNewOpinionAsync(Guid requesterId, Guid teleconsultoriaId, Guid opinionId, CancellationToken ct = default)
        => _hub.Clients.Group(requesterId.ToString())
            .SendAsync("NewOpinion", new { teleconsultoriaId, opinionId }, ct);
}

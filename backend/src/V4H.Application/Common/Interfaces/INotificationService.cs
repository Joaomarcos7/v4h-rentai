namespace V4H.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendNewOpinionAsync(Guid requesterId, Guid teleconsultoriaId, Guid opinionId, CancellationToken ct = default);
}

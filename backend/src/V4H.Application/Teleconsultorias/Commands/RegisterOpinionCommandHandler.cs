using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class RegisterOpinionCommandHandler : IRequestHandler<RegisterOpinionCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;
    private readonly IOpinionRepository _opinions;
    private readonly INotificationService _notifications;

    public RegisterOpinionCommandHandler(
        ITeleconsultoriaRepository repo,
        IOpinionRepository opinions,
        INotificationService notifications)
    {
        _repo = repo;
        _opinions = opinions;
        _notifications = notifications;
    }

    public async Task<Guid> Handle(RegisterOpinionCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        var opinion = Opinion.Create(tc.Id, request.SpecialistId, request.Content);
        await _opinions.AddAsync(opinion, cancellationToken);

        var oldStatus = tc.Status;
        tc.UpdateStatus(TeleconsultoriaStatus.Concluida);
        var history = StatusHistory.Create(tc.Id, oldStatus, TeleconsultoriaStatus.Concluida, request.SpecialistId);
        await _repo.AddStatusHistoryAsync(history, cancellationToken);

        await _repo.SaveChangesAsync(cancellationToken);
        await _opinions.SaveChangesAsync(cancellationToken);

        await _notifications.SendNewOpinionAsync(tc.RequesterId, tc.Id, opinion.Id, cancellationToken);

        return opinion.Id;
    }
}

using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand>
{
    private readonly ITeleconsultoriaRepository _repo;

    public UpdateStatusCommandHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.TeleconsultoriaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.TeleconsultoriaId);

        var history = StatusHistory.Create(
            tc.Id, tc.Status, request.NewStatus, request.ChangedById, request.Notes);

        tc.UpdateStatus(request.NewStatus);

        await _repo.AddStatusHistoryAsync(history, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}

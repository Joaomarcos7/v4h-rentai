using MediatR;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Commands;

public class CreateTeleconsultoriaCommandHandler : IRequestHandler<CreateTeleconsultoriaCommand, Guid>
{
    private readonly ITeleconsultoriaRepository _repo;

    public CreateTeleconsultoriaCommandHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateTeleconsultoriaCommand request, CancellationToken cancellationToken)
    {
        var tc = Teleconsultoria.Create(
            request.PatientName,
            request.BirthDate,
            request.Specialty,
            request.DiagnosticHypothesis,
            request.ClinicalHistory,
            request.RequesterId);

        await _repo.AddAsync(tc, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return tc.Id;
    }
}

using MediatR;
using V4H.Application.Common.Exceptions;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class GetTeleconsultoriaDetailQueryHandler
    : IRequestHandler<GetTeleconsultoriaDetailQuery, TeleconsultoriaDetailDto>
{
    private readonly ITeleconsultoriaRepository _repo;

    public GetTeleconsultoriaDetailQueryHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<TeleconsultoriaDetailDto> Handle(
        GetTeleconsultoriaDetailQuery request, CancellationToken cancellationToken)
    {
        var tc = await _repo.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teleconsultoria), request.Id);

        return new TeleconsultoriaDetailDto(
            tc.Id,
            tc.PatientName,
            tc.BirthDate,
            tc.Specialty.ToString(),
            tc.DiagnosticHypothesis,
            tc.ClinicalHistory,
            tc.Status.ToString(),
            tc.Requester?.Name ?? "",
            tc.CreatedAt,
            tc.UpdatedAt,
            tc.Documents.Select(d => new DocumentDto(
                d.Id, d.FileName, d.ValidationScore, d.IsApproved, d.ValidatedAt)).ToList(),
            tc.Opinions.Select(o => new OpinionDto(
                o.Id, o.Specialist?.Name ?? "", o.Content, o.CreatedAt)).ToList());
    }
}

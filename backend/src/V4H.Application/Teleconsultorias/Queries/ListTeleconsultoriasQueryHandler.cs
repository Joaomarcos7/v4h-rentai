using MediatR;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Teleconsultorias.Queries;

public class ListTeleconsultoriasQueryHandler
    : IRequestHandler<ListTeleconsultoriasQuery, List<TeleconsultoriaListItemDto>>
{
    private readonly ITeleconsultoriaRepository _repo;

    public ListTeleconsultoriasQueryHandler(ITeleconsultoriaRepository repo) => _repo = repo;

    public async Task<List<TeleconsultoriaListItemDto>> Handle(
        ListTeleconsultoriasQuery request, CancellationToken cancellationToken)
    {
        var list = await _repo.ListAsync(
            request.Specialty, request.Patient, request.Status,
            request.DateFrom, request.DateTo, cancellationToken);

        return list.Select(tc => new TeleconsultoriaListItemDto(
            tc.Id,
            tc.PatientName,
            tc.Specialty.ToString(),
            tc.Status.ToString(),
            tc.Requester?.Name ?? "",
            tc.CreatedAt)).ToList();
    }
}

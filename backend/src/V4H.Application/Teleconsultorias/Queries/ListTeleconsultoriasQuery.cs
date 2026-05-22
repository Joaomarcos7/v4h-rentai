using MediatR;
using V4H.Application.Teleconsultorias.DTOs;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Queries;

public record ListTeleconsultoriasQuery(
    Specialty? Specialty,
    string? Patient,
    TeleconsultoriaStatus? Status,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo) : IRequest<List<TeleconsultoriaListItemDto>>;

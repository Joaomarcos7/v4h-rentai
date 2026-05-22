using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Teleconsultorias.Commands;

public record CreateTeleconsultoriaCommand(
    string PatientName,
    DateOnly BirthDate,
    Specialty Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory,
    Guid RequesterId) : IRequest<Guid>;

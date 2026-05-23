namespace V4H.Application.Teleconsultorias.DTOs;

public record TeleconsultoriaDetailDto(
    Guid Id,
    string PatientName,
    DateOnly BirthDate,
    string Specialty,
    string DiagnosticHypothesis,
    string ClinicalHistory,
    string Status,
    string RequesterName,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<DocumentDto> Documents,
    List<OpinionDto> Opinions,
    List<StatusHistoryDto> StatusHistories);

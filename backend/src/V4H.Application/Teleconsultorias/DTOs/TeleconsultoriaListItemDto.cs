namespace V4H.Application.Teleconsultorias.DTOs;

public record TeleconsultoriaListItemDto(
    Guid Id,
    string PatientName,
    string Specialty,
    string Status,
    string RequesterName,
    DateTimeOffset CreatedAt);

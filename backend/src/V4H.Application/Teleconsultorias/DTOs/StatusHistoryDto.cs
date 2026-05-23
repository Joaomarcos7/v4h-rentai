namespace V4H.Application.Teleconsultorias.DTOs;

public record StatusHistoryDto(
    Guid Id,
    string OldStatus,
    string NewStatus,
    DateTimeOffset ChangedAt,
    string ChangedByName,
    string? Notes);

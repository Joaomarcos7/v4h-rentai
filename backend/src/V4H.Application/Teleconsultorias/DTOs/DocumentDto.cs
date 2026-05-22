namespace V4H.Application.Teleconsultorias.DTOs;

public record DocumentDto(
    Guid Id,
    string FileName,
    decimal ValidationScore,
    bool IsApproved,
    DateTimeOffset ValidatedAt);

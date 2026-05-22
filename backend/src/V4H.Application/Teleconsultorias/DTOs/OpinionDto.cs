namespace V4H.Application.Teleconsultorias.DTOs;

public record OpinionDto(
    Guid Id,
    string SpecialistName,
    string Content,
    DateTimeOffset CreatedAt);

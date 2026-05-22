namespace V4H.Application.Auth.DTOs;

public record AuthResultDto(string Token, Guid UserId, string Name, string Email, string Role);

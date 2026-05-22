using MediatR;
using V4H.Application.Auth.DTOs;

namespace V4H.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;

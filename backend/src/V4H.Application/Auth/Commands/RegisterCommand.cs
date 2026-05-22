using MediatR;
using V4H.Domain.Enums;

namespace V4H.Application.Auth.Commands;

public record RegisterCommand(string Name, string Email, string Password, UserRole Role) : IRequest<Guid>;

using MediatR;
using V4H.Application.Auth.DTOs;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var token = _jwt.Generate(user.Id, user.Email, user.Role.ToString());
        return new AuthResultDto(token, user.Id, user.Name, user.Email, user.Role.ToString());
    }
}

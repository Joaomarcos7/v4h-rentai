using MediatR;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterCommandHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Email '{request.Email}' already registered.");

        var hash = _hasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, hash, request.Role);

        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}

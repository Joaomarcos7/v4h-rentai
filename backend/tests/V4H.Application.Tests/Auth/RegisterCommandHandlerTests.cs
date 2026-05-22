using FluentAssertions;
using NSubstitute;
using V4H.Application.Auth.Commands;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();

    private RegisterCommandHandler CreateHandler() => new(_users, _hasher);

    [Fact]
    public async Task Handle_NewUser_ReturnsGuid()
    {
        _users.GetByEmailAsync("test@test.com").Returns((V4H.Domain.Entities.User?)null);
        _hasher.Hash("pass123").Returns("hashed");

        var cmd = new RegisterCommand("Joao", "test@test.com", "pass123", UserRole.Solicitante);
        var result = await CreateHandler().Handle(cmd, default);

        result.Should().NotBeEmpty();
        await _users.Received(1).AddAsync(Arg.Any<V4H.Domain.Entities.User>());
        await _users.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperation()
    {
        var existing = V4H.Domain.Entities.User.Create("X", "test@test.com", "h", UserRole.Solicitante);
        _users.GetByEmailAsync("test@test.com").Returns(existing);

        var cmd = new RegisterCommand("Y", "test@test.com", "pass", UserRole.Solicitante);
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}

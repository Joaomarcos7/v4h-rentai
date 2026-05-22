using FluentAssertions;
using NSubstitute;
using V4H.Application.Auth.Commands;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtService _jwt = Substitute.For<IJwtService>();

    private LoginCommandHandler CreateHandler() => new(_users, _hasher, _jwt);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = V4H.Domain.Entities.User.Create("Joao", "j@j.com", "hash", UserRole.Solicitante);
        _users.GetByEmailAsync("j@j.com").Returns(user);
        _hasher.Verify("pass", "hash").Returns(true);
        _jwt.Generate(user.Id, "j@j.com", "Solicitante").Returns("tok");

        var result = await CreateHandler().Handle(new LoginCommand("j@j.com", "pass"), default);

        result.Token.Should().Be("tok");
        result.Role.Should().Be("Solicitante");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var user = V4H.Domain.Entities.User.Create("X", "x@x.com", "hash", UserRole.Solicitante);
        _users.GetByEmailAsync("x@x.com").Returns(user);
        _hasher.Verify("wrong", "hash").Returns(false);

        var act = () => CreateHandler().Handle(new LoginCommand("x@x.com", "wrong"), default);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}

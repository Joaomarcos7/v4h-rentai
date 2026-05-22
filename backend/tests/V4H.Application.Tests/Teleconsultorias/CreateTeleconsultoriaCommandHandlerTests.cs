using FluentAssertions;
using NSubstitute;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class CreateTeleconsultoriaCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewId()
    {
        var cmd = new CreateTeleconsultoriaCommand(
            "Paciente X", new DateOnly(1990, 1, 1),
            Specialty.Cardiologia, "Hipotese", "Historia", Guid.NewGuid());

        var handler = new CreateTeleconsultoriaCommandHandler(_repo);
        var id = await handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(Arg.Any<V4H.Domain.Entities.Teleconsultoria>());
        await _repo.Received(1).SaveChangesAsync();
    }
}

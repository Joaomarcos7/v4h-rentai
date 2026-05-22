using FluentAssertions;
using NSubstitute;
using V4H.Application.Common.Interfaces;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class RegisterOpinionCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();
    private readonly IOpinionRepository _opinions = Substitute.For<IOpinionRepository>();
    private readonly INotificationService _notifications = Substitute.For<INotificationService>();

    private RegisterOpinionCommandHandler CreateHandler()
        => new(_repo, _opinions, _notifications);

    [Fact]
    public async Task Handle_ValidOpinion_UpdatesStatusAndNotifies()
    {
        var requesterId = Guid.NewGuid();
        var tc = Teleconsultoria.Create("P", new DateOnly(2000, 1, 1),
            Specialty.Cardiologia, "H", "C", requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);

        var specialistId = Guid.NewGuid();
        var cmd = new RegisterOpinionCommand(tc.Id, specialistId, "Parecer detalhado");
        var opinionId = await CreateHandler().Handle(cmd, default);

        opinionId.Should().NotBeEmpty();
        tc.Status.Should().Be(TeleconsultoriaStatus.Concluida);
        await _notifications.Received(1).SendNewOpinionAsync(requesterId, tc.Id, opinionId, default);
    }
}

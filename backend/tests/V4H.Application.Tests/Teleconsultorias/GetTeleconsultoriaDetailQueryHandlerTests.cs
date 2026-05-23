using FluentAssertions;
using NSubstitute;
using V4H.Application.Teleconsultorias.Queries;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class GetTeleconsultoriaDetailQueryHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();

    [Fact]
    public async Task Handle_ReturnsStatusHistories_MappedFromEntity()
    {
        var tcId = Guid.NewGuid();
        var changedById = Guid.NewGuid();

        var history = StatusHistory.Create(
            tcId,
            TeleconsultoriaStatus.Pendente,
            TeleconsultoriaStatus.EmAndamento,
            changedById,
            "Iniciando atendimento");

        // Set ChangedBy via reflection so handler can read Name
        var changedByUser = new { Name = "Dr. Ana" };
        var changedByField = typeof(StatusHistory).GetProperty("ChangedBy");

        var tc = Teleconsultoria.Create(
            "Paciente X", new DateOnly(1990, 1, 1),
            Specialty.Cardiologia, "Hipotese", "Historia", Guid.NewGuid());

        // Add history to the collection
        var historiesField = typeof(Teleconsultoria)
            .GetProperty("StatusHistories")!
            .GetValue(tc) as ICollection<StatusHistory>;
        historiesField!.Add(history);

        _repo.GetByIdWithDetailsAsync(tcId, default).Returns(tc);

        var handler = new GetTeleconsultoriaDetailQueryHandler(_repo);
        var result = await handler.Handle(new GetTeleconsultoriaDetailQuery(tcId), default);

        result.StatusHistories.Should().HaveCount(1);
        result.StatusHistories[0].OldStatus.Should().Be("Pendente");
        result.StatusHistories[0].NewStatus.Should().Be("EmAndamento");
        result.StatusHistories[0].Notes.Should().Be("Iniciando atendimento");
    }
}

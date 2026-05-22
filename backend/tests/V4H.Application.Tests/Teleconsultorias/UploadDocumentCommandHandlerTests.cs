using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using V4H.Application.Common.Exceptions;
using V4H.Application.Common.Interfaces;
using V4H.Application.Teleconsultorias.Commands;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;
using Xunit;

namespace V4H.Application.Tests.Teleconsultorias;

public class UploadDocumentCommandHandlerTests
{
    private readonly ITeleconsultoriaRepository _repo = Substitute.For<ITeleconsultoriaRepository>();
    private readonly IDocumentValidationService _validator = Substitute.For<IDocumentValidationService>();
    private readonly IFileStorageService _storage = Substitute.For<IFileStorageService>();

    private IConfiguration BuildConfig(decimal threshold = 0.6m)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["AI:ValidationThreshold"] = threshold.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }).Build();
    }

    private UploadDocumentCommandHandler CreateHandler(decimal threshold = 0.6m)
        => new(_repo, _validator, _storage, BuildConfig(threshold));

    private static Teleconsultoria MakeTc(Guid requesterId)
        => Teleconsultoria.Create("P", new DateOnly(2000, 1, 1),
            Specialty.Cardiologia, "H", "C", requesterId);

    [Fact]
    public async Task Handle_ScoreAboveThreshold_ReturnsDocumentId()
    {
        var requesterId = Guid.NewGuid();
        var tc = MakeTc(requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);
        _validator.ValidateAsync(Arg.Any<Stream>(), "application/pdf")
            .Returns(new ValidationResult(0.92m, "mock", DateTimeOffset.UtcNow));
        _storage.SaveAsync(Arg.Any<Stream>(), "test.pdf").Returns("/uploads/test.pdf");

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream(new byte[] { 1, 2, 3 }), "test.pdf", "application/pdf", requesterId);
        var docId = await CreateHandler().Handle(cmd, default);

        docId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ScoreBelowThreshold_ThrowsDocumentValidationException()
    {
        var requesterId = Guid.NewGuid();
        var tc = MakeTc(requesterId);
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);
        _validator.ValidateAsync(Arg.Any<Stream>(), "text/plain")
            .Returns(new ValidationResult(0.25m, "mock", DateTimeOffset.UtcNow));

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream(new byte[] { 1 }), "x.txt", "text/plain", requesterId);
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<DocumentValidationException>();
    }

    [Fact]
    public async Task Handle_WrongRequester_ThrowsUnauthorizedException()
    {
        var tc = MakeTc(Guid.NewGuid());
        _repo.GetByIdWithDetailsAsync(tc.Id).Returns(tc);

        var cmd = new UploadDocumentCommand(tc.Id, new MemoryStream(new byte[] { 1 }), "x.pdf", "application/pdf", Guid.NewGuid());
        var act = () => CreateHandler().Handle(cmd, default);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}

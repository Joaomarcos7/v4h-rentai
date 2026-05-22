using Microsoft.Extensions.Configuration;
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class MockDocumentValidationService : IDocumentValidationService
{
    private readonly decimal? _fixedScore;

    public MockDocumentValidationService(IConfiguration config)
    {
        var val = config["AI:MockScore"];
        _fixedScore = val is not null ? decimal.Parse(val, System.Globalization.CultureInfo.InvariantCulture) : null;
    }

    public Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default)
    {
        var score = _fixedScore ?? mimeType switch
        {
            "application/pdf" => 0.92m,
            "image/jpeg" or "image/png" => 0.78m,
            _ => 0.25m
        };

        return Task.FromResult(new ValidationResult(score, "MockValidator/1.0", DateTimeOffset.UtcNow));
    }
}

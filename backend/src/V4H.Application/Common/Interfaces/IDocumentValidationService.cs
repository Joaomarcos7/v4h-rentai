namespace V4H.Application.Common.Interfaces;

public record ValidationResult(decimal Score, string Provider, DateTimeOffset Timestamp);

public interface IDocumentValidationService
{
    Task<ValidationResult> ValidateAsync(Stream file, string mimeType, CancellationToken ct = default);
}

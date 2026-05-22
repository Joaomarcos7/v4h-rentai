namespace V4H.Domain.Entities;

public class TeleconsultoriaDocument
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public string FileName { get; private set; } = default!;
    public string StoredPath { get; private set; } = default!;
    public decimal ValidationScore { get; private set; }
    public string ValidationProvider { get; private set; } = default!;
    public decimal ValidationThreshold { get; private set; }
    public DateTimeOffset ValidatedAt { get; private set; }
    public bool IsApproved { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;

    private TeleconsultoriaDocument() { }

    public static TeleconsultoriaDocument Create(
        Guid teleconsultoriaId,
        string fileName,
        string storedPath,
        decimal score,
        string provider,
        decimal threshold,
        DateTimeOffset validatedAt)
    {
        return new TeleconsultoriaDocument
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            FileName = fileName,
            StoredPath = storedPath,
            ValidationScore = score,
            ValidationProvider = provider,
            ValidationThreshold = threshold,
            ValidatedAt = validatedAt,
            IsApproved = score >= threshold
        };
    }
}

namespace V4H.Domain.Entities;

public class Opinion
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public Guid SpecialistId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;
    public User Specialist { get; private set; } = default!;

    private Opinion() { }

    public static Opinion Create(Guid teleconsultoriaId, Guid specialistId, string content)
    {
        return new Opinion
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            SpecialistId = specialistId,
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}

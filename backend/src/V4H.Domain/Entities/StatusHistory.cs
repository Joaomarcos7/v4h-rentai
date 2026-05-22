using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class StatusHistory
{
    public Guid Id { get; private set; }
    public Guid TeleconsultoriaId { get; private set; }
    public TeleconsultoriaStatus OldStatus { get; private set; }
    public TeleconsultoriaStatus NewStatus { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public Guid ChangedById { get; private set; }
    public string? Notes { get; private set; }

    public Teleconsultoria Teleconsultoria { get; private set; } = default!;
    public User ChangedBy { get; private set; } = default!;

    private StatusHistory() { }

    public static StatusHistory Create(
        Guid teleconsultoriaId,
        TeleconsultoriaStatus oldStatus,
        TeleconsultoriaStatus newStatus,
        Guid changedById,
        string? notes = null)
    {
        return new StatusHistory
        {
            Id = Guid.NewGuid(),
            TeleconsultoriaId = teleconsultoriaId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTimeOffset.UtcNow,
            ChangedById = changedById,
            Notes = notes
        };
    }
}

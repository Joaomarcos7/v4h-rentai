using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class Teleconsultoria
{
    public Guid Id { get; private set; }
    public string PatientName { get; private set; } = default!;
    public DateOnly BirthDate { get; private set; }
    public Specialty Specialty { get; private set; }
    public string DiagnosticHypothesis { get; private set; } = default!;
    public string ClinicalHistory { get; private set; } = default!;
    public TeleconsultoriaStatus Status { get; private set; }
    public Guid RequesterId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public User Requester { get; private set; } = default!;
    public ICollection<TeleconsultoriaDocument> Documents { get; private set; } = new List<TeleconsultoriaDocument>();
    public ICollection<StatusHistory> StatusHistories { get; private set; } = new List<StatusHistory>();
    public ICollection<Opinion> Opinions { get; private set; } = new List<Opinion>();

    private Teleconsultoria() { }

    public static Teleconsultoria Create(
        string patientName,
        DateOnly birthDate,
        Specialty specialty,
        string diagnosticHypothesis,
        string clinicalHistory,
        Guid requesterId)
    {
        return new Teleconsultoria
        {
            Id = Guid.NewGuid(),
            PatientName = patientName,
            BirthDate = birthDate,
            Specialty = specialty,
            DiagnosticHypothesis = diagnosticHypothesis,
            ClinicalHistory = clinicalHistory,
            Status = TeleconsultoriaStatus.Pendente,
            RequesterId = requesterId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateStatus(TeleconsultoriaStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

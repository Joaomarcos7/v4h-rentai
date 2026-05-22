using V4H.Domain.Entities;
using V4H.Domain.Enums;

namespace V4H.Domain.Interfaces.Repositories;

public interface ITeleconsultoriaRepository
{
    Task<List<Teleconsultoria>> ListAsync(
        Specialty? specialty,
        string? patient,
        TeleconsultoriaStatus? status,
        DateTimeOffset? dateFrom,
        DateTimeOffset? dateTo,
        CancellationToken ct = default);

    Task<Teleconsultoria?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Teleconsultoria teleconsultoria, CancellationToken ct = default);
    Task AddDocumentAsync(TeleconsultoriaDocument document, CancellationToken ct = default);
    Task AddStatusHistoryAsync(StatusHistory history, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

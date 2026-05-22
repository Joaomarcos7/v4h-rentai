using Microsoft.EntityFrameworkCore;
using V4H.Domain.Entities;
using V4H.Domain.Enums;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Infrastructure.Persistence.Repositories;

public class TeleconsultoriaRepository : ITeleconsultoriaRepository
{
    private readonly AppDbContext _db;

    public TeleconsultoriaRepository(AppDbContext db) => _db = db;

    public async Task<List<Teleconsultoria>> ListAsync(
        Specialty? specialty, string? patient, TeleconsultoriaStatus? status,
        DateTimeOffset? dateFrom, DateTimeOffset? dateTo, CancellationToken ct = default)
    {
        var query = _db.Teleconsultorias.Include(t => t.Requester).AsQueryable();

        if (specialty.HasValue) query = query.Where(t => t.Specialty == specialty.Value);
        if (!string.IsNullOrWhiteSpace(patient))
            query = query.Where(t => t.PatientName.Contains(patient));
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (dateFrom.HasValue) query = query.Where(t => t.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(t => t.CreatedAt <= dateTo.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
    }

    public Task<Teleconsultoria?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        => _db.Teleconsultorias
            .Include(t => t.Requester)
            .Include(t => t.Documents)
            .Include(t => t.Opinions).ThenInclude(o => o.Specialist)
            .Include(t => t.StatusHistories)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(Teleconsultoria tc, CancellationToken ct = default)
        => await _db.Teleconsultorias.AddAsync(tc, ct);

    public async Task AddDocumentAsync(TeleconsultoriaDocument doc, CancellationToken ct = default)
        => await _db.Documents.AddAsync(doc, ct);

    public async Task AddStatusHistoryAsync(StatusHistory history, CancellationToken ct = default)
        => await _db.StatusHistories.AddAsync(history, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}

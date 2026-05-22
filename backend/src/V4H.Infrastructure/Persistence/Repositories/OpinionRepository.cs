using V4H.Domain.Entities;
using V4H.Domain.Interfaces.Repositories;

namespace V4H.Infrastructure.Persistence.Repositories;

public class OpinionRepository : IOpinionRepository
{
    private readonly AppDbContext _db;

    public OpinionRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Opinion opinion, CancellationToken ct = default)
        => await _db.Opinions.AddAsync(opinion, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}

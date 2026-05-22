using V4H.Domain.Entities;

namespace V4H.Domain.Interfaces.Repositories;

public interface IOpinionRepository
{
    Task AddAsync(Opinion opinion, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

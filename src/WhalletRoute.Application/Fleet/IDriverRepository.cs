using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Application.Fleet;

public interface IDriverRepository
{
    Task AddAsync(Driver driver, CancellationToken cancellationToken);
    Task<Driver?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Driver>> ListAsync(string tenantId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string tenantId, Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
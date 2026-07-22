using Microsoft.EntityFrameworkCore;
using WhalletRoute.Application.Fleet;
using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Infrastructure.Persistence;

public sealed class EfDriverRepository : IDriverRepository
{
    private readonly WhalletRouteDbContext _db;

    public EfDriverRepository(WhalletRouteDbContext db) => _db = db;

    public async Task AddAsync(Driver driver, CancellationToken cancellationToken)
        => await _db.Drivers.AddAsync(driver, cancellationToken);

    public async Task<Driver?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken)
        => await _db.Drivers.FirstOrDefaultAsync(d => d.TenantId == tenantId && d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Driver>> ListAsync(string tenantId, CancellationToken cancellationToken)
        => await _db.Drivers
            .Where(d => d.TenantId == tenantId)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(string tenantId, Guid id, CancellationToken cancellationToken)
        => _db.Drivers.AnyAsync(d => d.TenantId == tenantId && d.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _db.SaveChangesAsync(cancellationToken);
}
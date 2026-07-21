using Microsoft.EntityFrameworkCore;
using WhalletRoute.Application.Cargos;
using WhalletRoute.Domain.Deliveries;

namespace WhalletRoute.Infrastructure.Persistence;

public sealed class EfCargoRepository : ICargoRepository
{
    private readonly WhalletRouteDbContext _db;

    public EfCargoRepository(WhalletRouteDbContext db) => _db = db;

    public async Task AddAsync(Cargo cargo, CancellationToken cancellationToken)
        => await _db.Cargos.AddAsync(cargo, cancellationToken);

    public async Task<Cargo?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken)
        => await _db.Cargos.FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Cargo>> ListAsync(string tenantId, CancellationToken cancellationToken)
        => await _db.Cargos
            .Where(c => c.TenantId == tenantId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _db.SaveChangesAsync(cancellationToken);
}
using Microsoft.EntityFrameworkCore;
using WhalletRoute.Application.Fleet;
using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Infrastructure.Persistence;

public sealed class EfVehicleRepository : IVehicleRepository
{
    private readonly WhalletRouteDbContext _db;

    public EfVehicleRepository(WhalletRouteDbContext db) => _db = db;

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken)
        => await _db.Vehicles.AddAsync(vehicle, cancellationToken);

    public async Task<Vehicle?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken)
        => await _db.Vehicles.FirstOrDefaultAsync(v => v.TenantId == tenantId && v.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Vehicle>> ListAsync(string tenantId, CancellationToken cancellationToken)
        => await _db.Vehicles
            .Where(v => v.TenantId == tenantId)
            .OrderBy(v => v.Plate)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(string tenantId, Guid id, CancellationToken cancellationToken)
        => _db.Vehicles.AnyAsync(v => v.TenantId == tenantId && v.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _db.SaveChangesAsync(cancellationToken);
}
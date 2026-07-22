using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Application.Fleet;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken);
    Task<Vehicle?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Vehicle>> ListAsync(string tenantId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string tenantId, Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
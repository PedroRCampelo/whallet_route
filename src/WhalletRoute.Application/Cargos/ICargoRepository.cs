using WhalletRoute.Domain.Deliveries;

namespace WhalletRoute.Application.Cargos;

public interface ICargoRepository
{
    Task AddAsync(Cargo cargo, CancellationToken cancellationToken);
    Task<Cargo?> GetByIdAsync(string tenantId, Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Cargo>> ListAsync(string tenantId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
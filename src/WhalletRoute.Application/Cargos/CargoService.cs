using WhalletRoute.Application.Cargos.Contracts;
using WhalletRoute.Domain.Deliveries;
using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Application.Cargos;

public sealed class CargoService
{
    private readonly ICargoRepository _repository;

    public CargoService(ICargoRepository repository) => _repository = repository;

    public async Task<CargoResponse> CreateAsync(string tenantId, CreateCargoRequest request, CancellationToken cancellationToken)
    {
        if (request.Deliveries.Count == 0)
            throw new ArgumentException("A cargo must have at least one delivery.");

        var cargo = new Cargo(tenantId, request.ExternalId, request.OriginAddress);

        if (request.OriginLatitude is not null && request.OriginLongitude is not null)
            cargo.SetOriginCoordinate(new Coordinate(request.OriginLatitude.Value, request.OriginLongitude.Value));

        if (request.DriverId is not null)
            cargo.AssignDriver(request.DriverId.Value);

        if (request.VehicleId is not null)
            cargo.AssignVehicle(request.VehicleId.Value);

        foreach (var item in request.Deliveries)
            cargo.AddDelivery(ToDelivery(item));

        await _repository.AddAsync(cargo, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> GetAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, id, cancellationToken);
        return cargo is null ? null : ToResponse(cargo);
    }

    public async Task<IReadOnlyList<CargoSummaryResponse>> ListAsync(string tenantId, CancellationToken cancellationToken)
    {
        var cargos = await _repository.ListAsync(tenantId, cancellationToken);

        return cargos
            .Select(c => new CargoSummaryResponse
            {
                Id = c.Id,
                ExternalId = c.ExternalId,
                Status = c.Status.ToString(),
                DeliveryCount = c.Deliveries.Count,
                CreatedAt = c.CreatedAt
            })
            .ToList();
    }

    private static Delivery ToDelivery(CreateDeliveryRequest request)
    {
        DeliveryWindow? window = null;
        if (request.WindowFrom is not null && request.WindowTo is not null)
            window = new DeliveryWindow(request.WindowFrom.Value, request.WindowTo.Value);

        var delivery = new Delivery(
            request.ExternalId, request.ClientId, request.ClientName, request.Address,
            request.WeightKg, request.VolumeM3, window, request.Phone, request.Instructions);

        if (request.Latitude is not null && request.Longitude is not null)
            delivery.SetCoordinate(new Coordinate(request.Latitude.Value, request.Longitude.Value));

        return delivery;
    }

    private static CargoResponse ToResponse(Cargo cargo) => new()
    {
        Id = cargo.Id,
        ExternalId = cargo.ExternalId,
        OriginAddress = cargo.OriginAddress,
        Status = cargo.Status.ToString(),
        DriverId = cargo.DriverId,
        VehicleId = cargo.VehicleId,
        CreatedAt = cargo.CreatedAt,
        Deliveries = cargo.Deliveries
            .Select(d => new DeliveryResponse
            {
                Id = d.Id,
                ExternalId = d.ExternalId,
                ClientId = d.ClientId,
                ClientName = d.ClientName,
                Address = d.Address,
                Latitude = d.Coordinate?.Latitude,
                Longitude = d.Coordinate?.Longitude,
                Order = d.Order,
                Status = d.Status.ToString()
            })
            .ToList()
    };
}
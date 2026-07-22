using WhalletRoute.Application.Cargos.Contracts;
using WhalletRoute.Application.Fleet;
using WhalletRoute.Application.Geocoding;
using WhalletRoute.Application.Routing;
using WhalletRoute.Domain.Deliveries;
using WhalletRoute.Domain.Geo;
using WhalletRoute.Domain.Routing;

namespace WhalletRoute.Application.Cargos;

public sealed class CargoService
{
    private readonly ICargoRepository _repository;
    
    private readonly IGeocoder _geocoder;
    private readonly IRouteSolver _solver;
    private readonly IDriverRepository _drivers;
    private readonly IVehicleRepository _vehicles;

    public CargoService(
        ICargoRepository repository, IGeocoder geocoder, IRouteSolver solver,
        IDriverRepository drivers, IVehicleRepository vehicles)
    {
        _repository = repository;
        _geocoder = geocoder;
        _solver = solver;
        _drivers = drivers;
        _vehicles = vehicles;
    }

    public async Task<CargoResponse> CreateAsync(string tenantId, CreateCargoRequest request, CancellationToken cancellationToken)
    {
        if (request.Deliveries.Count == 0)
            throw new ArgumentException("A cargo must have at least one delivery.");

        var cargo = new Cargo(tenantId, request.ExternalId, request.OriginAddress);

        if (request.OriginLatitude is not null && request.OriginLongitude is not null)
            cargo.SetOriginCoordinate(new Coordinate(request.OriginLatitude.Value, request.OriginLongitude.Value));

        if (request.DriverId is not null)
        {
            if (!await _drivers.ExistsAsync(tenantId, request.DriverId.Value, cancellationToken))
                throw new ArgumentException("Driver not found.");
            cargo.AssignDriver(request.DriverId.Value);
        }

        if (request.VehicleId is not null)
        {
            if (!await _vehicles.ExistsAsync(tenantId, request.VehicleId.Value, cancellationToken))
                throw new ArgumentException("Vehicle not found.");
            cargo.AssignVehicle(request.VehicleId.Value);
        }

        foreach (var item in request.Deliveries)
            cargo.AddDelivery(ToDelivery(item));

        await _repository.AddAsync(cargo, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> RouteAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, id, cancellationToken);
        if (cargo is null)
            return null;

        if (cargo.OriginCoordinate is null)
            cargo.SetOriginCoordinate(await _geocoder.GeocodeAsync(cargo.OriginAddress, cancellationToken));

        foreach (var delivery in cargo.Deliveries.Where(d => d.Coordinate is null))
            delivery.SetCoordinate(await _geocoder.GeocodeAsync(delivery.Address, cancellationToken));

        var origin = new Stop(cargo.Id.ToString(), cargo.OriginCoordinate!);
        var stops = cargo.Deliveries
            .Select(d => new Stop(d.Id.ToString(), d.Coordinate!))
            .ToList();

        var route = _solver.Solve(origin, stops);

        foreach (var orderedStop in route.Stops)
        {
            var delivery = cargo.Deliveries.First(d => d.Id.ToString() == orderedStop.StopId);
            delivery.SetOrder(orderedStop.Order);
        }

        cargo.MarkRouted();
        await _repository.SaveChangesAsync(cancellationToken);

        return ToResponse(cargo);
    }
    
    public async Task<CargoResponse?> DispatchAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, id, cancellationToken);
        if (cargo is null)
            return null;

        cargo.Dispatch();

        foreach (var delivery in cargo.Deliveries)
            delivery.MarkEnRoute();

        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> CancelDispatchAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, id, cancellationToken);
        if (cargo is null)
            return null;

        cargo.CancelDispatch();
        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> DeliverAsync(string tenantId, Guid cargoId, Guid deliveryId, DeliverRequest request, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, cargoId, cancellationToken);
        if (cargo is null)
            return null;

        var delivery = cargo.Deliveries.FirstOrDefault(d => d.Id == deliveryId);
        if (delivery is null)
            return null;

        delivery.MarkDelivered(ToCoordinate(request.Latitude, request.Longitude), request.Note);
        CompleteIfFinished(cargo);

        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> RefuseAsync(string tenantId, Guid cargoId, Guid deliveryId, RefuseDeliveryRequest request, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, cargoId, cancellationToken);
        if (cargo is null)
            return null;

        var delivery = cargo.Deliveries.FirstOrDefault(d => d.Id == deliveryId);
        if (delivery is null)
            return null;

        delivery.MarkRefused(request.Reason, ToCoordinate(request.Latitude, request.Longitude));
        CompleteIfFinished(cargo);

        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
    }

    private static Coordinate? ToCoordinate(double? latitude, double? longitude)
        => latitude is not null && longitude is not null
            ? new Coordinate(latitude.Value, longitude.Value)
            : null;

    private static void CompleteIfFinished(Cargo cargo)
    {
        var allFinished = cargo.Deliveries.All(d =>
            d.Status is DeliveryStatus.Delivered or DeliveryStatus.Refused);

        if (allFinished && cargo.Status == CargoStatus.Dispatched)
            cargo.Complete();
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
    
    public async Task<CargoResponse?> AssignDriverAsync(string tenantId, Guid cargoId, Guid driverId, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, cargoId, cancellationToken);
        if (cargo is null)
            return null;

        if (!await _drivers.ExistsAsync(tenantId, driverId, cancellationToken))
            throw new ArgumentException("Driver not found.");

        cargo.AssignDriver(driverId);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
    }

    public async Task<CargoResponse?> AssignVehicleAsync(string tenantId, Guid cargoId, Guid vehicleId, CancellationToken cancellationToken)
    {
        var cargo = await _repository.GetByIdAsync(tenantId, cargoId, cancellationToken);
        if (cargo is null)
            return null;

        if (!await _vehicles.ExistsAsync(tenantId, vehicleId, cancellationToken))
            throw new ArgumentException("Vehicle not found.");

        cargo.AssignVehicle(vehicleId);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(cargo);
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
            .OrderBy(d => d.Order ?? int.MaxValue)
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
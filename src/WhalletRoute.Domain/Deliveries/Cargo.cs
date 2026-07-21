using WhalletRoute.Domain.Geo;

namespace WhalletRoute.Domain.Deliveries;

public sealed class Cargo
{
    private readonly List<Delivery> _deliveries = new();

    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string ExternalId { get; private set; } = null!;
    public string OriginAddress { get; private set; } = null!;
    public Coordinate? OriginCoordinate { get; private set; }
    public CargoStatus Status { get; private set; }
    public Guid? DriverId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyList<Delivery> Deliveries => _deliveries;

    private Cargo() { }
    
    public Cargo(string tenantId, string externalId, string originAddress)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant id is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("External id is required.", nameof(externalId));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ExternalId = externalId;
        OriginAddress = originAddress;
        Status = CargoStatus.Received;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetOriginCoordinate(Coordinate coordinate) => OriginCoordinate = coordinate;

    public void AddDelivery(Delivery delivery)
    {
        EnsureEditable();
        _deliveries.Add(delivery);
        InvalidateRoute();
    }

    public void RemoveDelivery(Guid deliveryId)
    {
        EnsureEditable();
        var delivery = _deliveries.FirstOrDefault(d => d.Id == deliveryId);
        if (delivery is null)
            throw new InvalidOperationException("Delivery not found in this cargo.");

        _deliveries.Remove(delivery);
        InvalidateRoute();
    }

    public void AssignDriver(Guid driverId) => DriverId = driverId;

    public void AssignVehicle(Guid vehicleId) => VehicleId = vehicleId;

    public void MarkRouted()
    {
        if (Status != CargoStatus.Received)
            throw new InvalidOperationException($"Cannot route a cargo that is {Status}.");
        if (_deliveries.Count == 0)
            throw new InvalidOperationException("Cannot route a cargo with no deliveries.");

        Status = CargoStatus.Routed;
    }

    public void Dispatch()
    {
        if (Status != CargoStatus.Routed)
            throw new InvalidOperationException($"Cannot dispatch a cargo that is {Status}.");
        if (DriverId is null)
            throw new InvalidOperationException("Assign a driver before dispatching.");

        Status = CargoStatus.Dispatched;
    }

    public void CancelDispatch()
    {
        if (Status != CargoStatus.Dispatched)
            throw new InvalidOperationException($"Cannot cancel dispatch of a cargo that is {Status}.");

        Status = CargoStatus.Routed;
    }

    public void Complete()
    {
        if (Status != CargoStatus.Dispatched)
            throw new InvalidOperationException($"Cannot complete a cargo that is {Status}.");

        Status = CargoStatus.Completed;
    }

    private void EnsureEditable()
    {
        if (Status is CargoStatus.Dispatched or CargoStatus.Completed)
            throw new InvalidOperationException($"Cannot edit a cargo that is {Status}.");
    }

    private void InvalidateRoute()
    {
        if (Status == CargoStatus.Routed)
            Status = CargoStatus.Received;
    }
}
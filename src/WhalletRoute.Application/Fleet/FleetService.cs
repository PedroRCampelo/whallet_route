using WhalletRoute.Application.Fleet.Contracts;
using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Application.Fleet;

public sealed class FleetService
{
    private readonly IDriverRepository _drivers;
    private readonly IVehicleRepository _vehicles;

    public FleetService(IDriverRepository drivers, IVehicleRepository vehicles)
    {
        _drivers = drivers;
        _vehicles = vehicles;
    }

    public async Task<DriverResponse> CreateDriverAsync(string tenantId, CreateDriverRequest request, CancellationToken cancellationToken)
    {
        var driver = new Driver(
            tenantId, request.Name, request.Document, request.Phone,
            request.LicenseNumber, request.LicenseCategory, request.LicenseExpiry);

        await _drivers.AddAsync(driver, cancellationToken);
        await _drivers.SaveChangesAsync(cancellationToken);

        return ToResponse(driver);
    }

    public async Task<DriverResponse?> GetDriverAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var driver = await _drivers.GetByIdAsync(tenantId, id, cancellationToken);
        return driver is null ? null : ToResponse(driver);
    }

    public async Task<IReadOnlyList<DriverResponse>> ListDriversAsync(string tenantId, CancellationToken cancellationToken)
    {
        var drivers = await _drivers.ListAsync(tenantId, cancellationToken);
        return drivers.Select(ToResponse).ToList();
    }

    public async Task<VehicleResponse> CreateVehicleAsync(string tenantId, CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle(
            tenantId, request.Plate, request.CapacityKg, request.CapacityM3, request.Description);

        await _vehicles.AddAsync(vehicle, cancellationToken);
        await _vehicles.SaveChangesAsync(cancellationToken);

        return ToResponse(vehicle);
    }

    public async Task<VehicleResponse?> GetVehicleAsync(string tenantId, Guid id, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicles.GetByIdAsync(tenantId, id, cancellationToken);
        return vehicle is null ? null : ToResponse(vehicle);
    }

    public async Task<IReadOnlyList<VehicleResponse>> ListVehiclesAsync(string tenantId, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicles.ListAsync(tenantId, cancellationToken);
        return vehicles.Select(ToResponse).ToList();
    }

    private static DriverResponse ToResponse(Driver driver) => new()
    {
        Id = driver.Id,
        Name = driver.Name,
        Document = driver.Document,
        Phone = driver.Phone,
        LicenseNumber = driver.LicenseNumber,
        LicenseCategory = driver.LicenseCategory,
        LicenseExpiry = driver.LicenseExpiry
    };

    private static VehicleResponse ToResponse(Vehicle vehicle) => new()
    {
        Id = vehicle.Id,
        Plate = vehicle.Plate,
        CapacityKg = vehicle.CapacityKg,
        CapacityM3 = vehicle.CapacityM3,
        Description = vehicle.Description
    };
}
namespace WhalletRoute.Domain.Fleet;

public sealed class Driver
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Document { get; private set; }
    public string? Phone { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? LicenseCategory { get; private set; }
    public DateOnly? LicenseExpiry { get; private set; }

    private Driver() { }
    
    public Driver(
        string tenantId, string name,
        string? document = null, string? phone = null,
        string? licenseNumber = null, string? licenseCategory = null, DateOnly? licenseExpiry = null)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant id is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Driver name is required.", nameof(name));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        Document = document;
        Phone = phone;
        LicenseNumber = licenseNumber;
        LicenseCategory = licenseCategory;
        LicenseExpiry = licenseExpiry;
    }
}
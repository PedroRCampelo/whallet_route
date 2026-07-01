namespace WhalletRoute.Domain.Fleet;

public sealed class Driver
{
    public Guid Id { get; }
    public string TenantId { get; }
    public string Name { get; }
    public string? Document { get; }
    public string? Phone { get; }
    public string? LicenseNumber { get; }
    public string? LicenseCategory { get; }
    public DateOnly? LicenseExpiry { get; }

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
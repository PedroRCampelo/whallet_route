namespace WhalletRoute.Domain.Tenancy;

public sealed class ApiKey
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string KeyHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private ApiKey() { }

    public ApiKey(string tenantId, string keyHash)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant id is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(keyHash))
            throw new ArgumentException("Key hash is required.", nameof(keyHash));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        KeyHash = keyHash;
        CreatedAt = DateTime.UtcNow;
    }
}
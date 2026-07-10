namespace WhalletRoute.Domain.Tenancy;

public sealed class ApiKey
{
    public Guid Id { get; }
    public string TenantId { get; }
    public string KeyHash { get; }
    public DateTime CreatedAt { get; }

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
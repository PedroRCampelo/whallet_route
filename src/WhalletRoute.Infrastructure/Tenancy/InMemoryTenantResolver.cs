using WhalletRoute.Application.Tenancy;
using WhalletRoute.Domain.Tenancy;

namespace WhalletRoute.Infrastructure.Tenancy;

public sealed class InMemoryTenantResolver : ITenantResolver
{
    private readonly Dictionary<string, Tenant> _tenantsByKeyHash;

    public InMemoryTenantResolver()
    {
        var demo = new Tenant("tenant-demo", "Tenant Demo");
        _tenantsByKeyHash = new Dictionary<string, Tenant>
        {
            [ApiKeyHasher.Hash("wr_dev_local_key_123")] = demo
        };
    }

    public Task<Tenant?> ResolveByApiKeyAsync(string rawApiKey, CancellationToken cancellationToken)
    {
        var hash = ApiKeyHasher.Hash(rawApiKey);
        _tenantsByKeyHash.TryGetValue(hash, out var tenant);
        return Task.FromResult(tenant);
    }
}
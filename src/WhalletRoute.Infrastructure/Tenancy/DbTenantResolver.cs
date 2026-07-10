using Microsoft.EntityFrameworkCore;
using WhalletRoute.Application.Tenancy;
using WhalletRoute.Domain.Tenancy;
using WhalletRoute.Infrastructure.Persistence;

namespace WhalletRoute.Infrastructure.Tenancy;

public sealed class DbTenantResolver : ITenantResolver
{
    private readonly WhalletRouteDbContext _db;

    public DbTenantResolver(WhalletRouteDbContext db) => _db = db;

    public async Task<Tenant?> ResolveByApiKeyAsync(string rawApiKey, CancellationToken cancellationToken)
    {
        var hash = ApiKeyHasher.Hash(rawApiKey);

        var apiKey = await _db.ApiKeys.FirstOrDefaultAsync(k => k.KeyHash == hash, cancellationToken);
        if (apiKey is null)
            return null;

        return await _db.Tenants.FirstOrDefaultAsync(t => t.Id == apiKey.TenantId, cancellationToken);
    }
}
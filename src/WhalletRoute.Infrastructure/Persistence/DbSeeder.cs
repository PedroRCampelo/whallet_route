using Microsoft.EntityFrameworkCore;
using WhalletRoute.Domain.Tenancy;
using WhalletRoute.Infrastructure.Tenancy;

namespace WhalletRoute.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(WhalletRouteDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Tenants.AnyAsync(cancellationToken))
            return;

        var tenant = new Tenant("tenant-demo", "Tenant Demo");
        db.Tenants.Add(tenant);
        db.ApiKeys.Add(new ApiKey(tenant.Id, ApiKeyHasher.Hash("wr_dev_local_key_123")));

        await db.SaveChangesAsync(cancellationToken);
    }
}
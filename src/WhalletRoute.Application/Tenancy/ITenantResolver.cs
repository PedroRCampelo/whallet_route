using WhalletRoute.Domain.Tenancy;

namespace WhalletRoute.Application.Tenancy;

public interface ITenantResolver
{
    Task<Tenant?> ResolveByApiKeyAsync(string rawApiKey, CancellationToken cancellationToken);
}
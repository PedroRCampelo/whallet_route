using WhalletRoute.Application.Tenancy;
using WhalletRoute.Domain.Tenancy;

namespace WhalletRoute.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public Tenant? Current { get; private set; }
    public void Set(Tenant tenant) => Current = tenant;
}
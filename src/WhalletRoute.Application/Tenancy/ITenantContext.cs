using WhalletRoute.Domain.Tenancy;

namespace WhalletRoute.Application.Tenancy;

public interface ITenantContext
{
    Tenant? Current { get; }
    void Set(Tenant tenant);
}
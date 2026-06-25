namespace WhalletRoute.Domain.Tenancy;

public sealed class Tenant
{
    public string Id { get; }
    public string Name { get; }

    public Tenant(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Tenant id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required.", nameof(name));

        Id = id;
        Name = name;
    }
}
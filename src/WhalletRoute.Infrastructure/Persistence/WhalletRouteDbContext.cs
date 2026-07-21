using Microsoft.EntityFrameworkCore;
using WhalletRoute.Domain.Deliveries;
using WhalletRoute.Domain.Fleet;
using WhalletRoute.Domain.Tenancy;
using WhalletRoute.Infrastructure.Persistence.Configurations;

namespace WhalletRoute.Infrastructure.Persistence;

public sealed class WhalletRouteDbContext : DbContext
{
    public WhalletRouteDbContext(DbContextOptions<WhalletRouteDbContext> options) : base(options) { }

    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CargoConfiguration());

        modelBuilder.Entity<Driver>(b =>
        {
            b.ToTable("drivers");
            b.HasKey(d => d.Id);
            b.Property(d => d.TenantId).IsRequired();
            b.Property(d => d.Name).IsRequired();
            b.Property(d => d.Document);
            b.Property(d => d.Phone);
            b.Property(d => d.LicenseNumber);
            b.Property(d => d.LicenseCategory);
            b.Property(d => d.LicenseExpiry);
        });

        modelBuilder.Entity<Vehicle>(b =>
        {
            b.ToTable("vehicles");
            b.HasKey(v => v.Id);
            b.Property(v => v.TenantId).IsRequired();
            b.Property(v => v.Plate).IsRequired();
            b.Property(v => v.CapacityKg);
            b.Property(v => v.CapacityM3);
            b.Property(v => v.Description);
        });

        modelBuilder.Entity<Tenant>(b =>
        {
            b.ToTable("tenants");
            b.HasKey(t => t.Id);
            b.Property(t => t.Id).ValueGeneratedNever();
            b.Property(t => t.Name).IsRequired();
        });

        modelBuilder.Entity<ApiKey>(b =>
        {
            b.ToTable("api_keys");
            b.HasKey(k => k.Id);
            b.Property(k => k.TenantId).IsRequired();
            b.Property(k => k.KeyHash).IsRequired();
            b.HasIndex(k => k.KeyHash).IsUnique();
        });
    }
}
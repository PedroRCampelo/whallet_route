using Microsoft.EntityFrameworkCore;
using WhalletRoute.Domain.Fleet;

namespace WhalletRoute.Infrastructure.Persistence;

public sealed class WhalletRouteDbContext : DbContext
{
    public WhalletRouteDbContext(DbContextOptions<WhalletRouteDbContext> options) : base(options) { }

    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>(b =>
        {
            b.ToTable("drivers");
            b.HasKey(d => d.Id);
            b.Property(d => d.TenantId).IsRequired();
            b.Property(d => d.Name).IsRequired();
        });

        modelBuilder.Entity<Vehicle>(b =>
        {
            b.ToTable("vehicles");
            b.HasKey(v => v.Id);
            b.Property(v => v.TenantId).IsRequired();
            b.Property(v => v.Plate).IsRequired();
        });
    }
}
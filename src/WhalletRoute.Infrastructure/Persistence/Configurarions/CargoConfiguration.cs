using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhalletRoute.Domain.Deliveries;

namespace WhalletRoute.Infrastructure.Persistence.Configurations;

public sealed class CargoConfiguration : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> builder)
    {
        builder.ToTable("cargos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId).IsRequired();
        builder.Property(c => c.ExternalId).IsRequired();
        builder.Property(c => c.OriginAddress).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>();

        builder.OwnsOne(c => c.OriginCoordinate, o =>
        {
            o.Property(p => p.Latitude).HasColumnName("origin_latitude");
            o.Property(p => p.Longitude).HasColumnName("origin_longitude");
        });

        builder.Metadata
            .FindNavigation(nameof(Cargo.Deliveries))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(c => c.Deliveries, DeliveryConfiguration.Configure);
    }
}
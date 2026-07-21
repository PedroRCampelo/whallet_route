using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhalletRoute.Domain.Deliveries;

namespace WhalletRoute.Infrastructure.Persistence.Configurations;

public static class DeliveryConfiguration
{
    public static void Configure(OwnedNavigationBuilder<Cargo, Delivery> builder)
    {
        builder.ToTable("deliveries");
        builder.WithOwner().HasForeignKey("CargoId");
        builder.HasKey("Id");

        builder.Property(d => d.ExternalId).IsRequired();
        builder.Property(d => d.ClientId).IsRequired();
        builder.Property(d => d.Status).HasConversion<string>();
        
        builder.Property(d => d.ClientName).IsRequired();
        builder.Property(d => d.Address).IsRequired();
        builder.Property(d => d.WeightKg);
        builder.Property(d => d.VolumeM3);
        builder.Property(d => d.Phone);
        builder.Property(d => d.Instructions);

        builder.OwnsOne(d => d.Coordinate, o =>
        {
            o.Property(p => p.Latitude).HasColumnName("latitude");
            o.Property(p => p.Longitude).HasColumnName("longitude");
        });

        builder.OwnsOne(d => d.Window, w =>
        {
            w.Property(p => p.From).HasColumnName("window_from");
            w.Property(p => p.To).HasColumnName("window_to");
        });

        // builder.Metadata
        //     .FindNavigation(nameof(Delivery.History))!
        //     .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(d => d.History, h =>
        {
            h.ToTable("delivery_status_history");
            h.WithOwner().HasForeignKey("DeliveryId");
            h.Property(s => s.To).HasConversion<string>();

            h.OwnsOne(s => s.Location, o =>
            {
                o.Property(p => p.Latitude).HasColumnName("latitude");
                o.Property(p => p.Longitude).HasColumnName("longitude");
            });
        });
        
    }
}
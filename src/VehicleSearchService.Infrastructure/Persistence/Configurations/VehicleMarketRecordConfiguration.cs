using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence.Configurations;

public sealed class VehicleMarketRecordConfiguration : IEntityTypeConfiguration<VehicleMarketRecord>
{
    public void Configure(EntityTypeBuilder<VehicleMarketRecord> builder)
    {
        builder.ToTable("vehicle_markets");
        builder.HasKey(x => new { x.VehicleId, x.MarketCode });
        builder.Property(x => x.MarketCode).HasMaxLength(64);
        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.EnabledMarkets)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

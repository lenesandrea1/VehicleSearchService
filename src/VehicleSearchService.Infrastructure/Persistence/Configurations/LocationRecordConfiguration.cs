using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence.Configurations;

public sealed class LocationRecordConfiguration : IEntityTypeConfiguration<LocationRecord>
{
    public void Configure(EntityTypeBuilder<LocationRecord> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MarketId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(256);
    }
}

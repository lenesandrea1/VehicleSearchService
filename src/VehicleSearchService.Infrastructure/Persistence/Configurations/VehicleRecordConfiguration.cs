using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence.Configurations;

public sealed class VehicleRecordConfiguration : IEntityTypeConfiguration<VehicleRecord>
{
    public void Configure(EntityTypeBuilder<VehicleRecord> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VehicleTypeCatalogId).HasMaxLength(64);
        builder.HasIndex(x => x.LocationId);
        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleSearchService.Infrastructure.Persistence.Entities;

namespace VehicleSearchService.Infrastructure.Persistence.Configurations;

public sealed class ReservationRecordConfiguration : IEntityTypeConfiguration<ReservationRecord>
{
    public void Configure(EntityTypeBuilder<ReservationRecord> builder)
    {
        builder.ToTable("reservations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.VehicleId);
        builder.Property(x => x.PickupAtUtc).HasColumnType("datetime(6)");
        builder.Property(x => x.ReturnAtUtc).HasColumnType("datetime(6)");
    }
}

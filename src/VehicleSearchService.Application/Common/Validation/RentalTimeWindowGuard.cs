using VehicleSearchService.Application.Common.Exceptions;

namespace VehicleSearchService.Application.Common.Validation;

/// <summary>Shared rules for search and reservation: half-open window [pickup, return) must be in the future at pickup.</summary>
public static class RentalTimeWindowGuard
{
    public static void EnsureValidForRequest(DateTime pickupAtUtc, DateTime returnAtUtc, DateTime utcNow)
    {
        var pickup = NormalizeToUtc(pickupAtUtc);
        var drop = NormalizeToUtc(returnAtUtc);
        var now = NormalizeToUtc(utcNow);

        if (drop <= pickup)
        {
            throw new InvalidRentalTimeWindowException(
                "Return date and time must be after pickup date and time.");
        }

        if (pickup < now)
        {
            throw new InvalidRentalTimeWindowException(
                "Pickup date and time cannot be in the past.");
        }
    }

    private static DateTime NormalizeToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}

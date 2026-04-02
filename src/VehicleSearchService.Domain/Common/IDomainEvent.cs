namespace VehicleSearchService.Domain.Common;

/// <summary>Evento de dominio publicado tras un cambio significativo en el modelo.</summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

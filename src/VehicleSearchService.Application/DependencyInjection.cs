using Microsoft.Extensions.DependencyInjection;
using VehicleSearchService.Application.Features.Reservations;
using VehicleSearchService.Application.Features.VehicleSearch;

namespace VehicleSearchService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISearchVehiclesQueryHandler, SearchVehiclesQueryHandler>();
        services.AddScoped<ICreateReservationCommandHandler, CreateReservationCommandHandler>();
        return services;
    }
}

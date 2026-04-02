# VehicleSearchService

Web API de búsqueda y reserva de vehículos para el reto técnico **Outlet Rental Cars**, con **Clean Architecture**, dominio con comportamiento explícito y contratos de aplicación separados de la infraestructura.

## Estructura

| Proyecto | Rol |
|----------|-----|
| `VehicleSearchService.Domain` | Entidades, reglas de negocio, eventos de dominio |
| `VehicleSearchService.Application` | Puertos (persistencia, mensajería), queries/commands, DTOs |
| `VehicleSearchService.Infrastructure` | Implementaciones (MySQL, MongoDB, publicador de eventos) |
| `VehicleSearchService.Api` | Host HTTP, composición e inyección de dependencias |
| `VehicleSearchService.Tests.Unit` | Especificaciones del dominio |
| `VehicleSearchService.Tests.Integration` | API y contratos HTTP |

## Convenciones de dominio

- Periodos de alquiler en **UTC**, intervalo semiabierto `[inicio, fin)`.
- La **localidad de devolución** puede diferir de la de recogida; la disponibilidad en búsqueda se evalúa respecto a la **recogida** y al **mercado** de esa localidad.
- Las reservas **pendientes** o **confirmadas** bloquean solapamientos; **canceladas** y **completadas** no.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (o superior con `rollForward` definido en `global.json`).

## Compilar y probar

```bash
dotnet build
dotnet test
```

Ejecutar la API:

```bash
dotnet run --project src/VehicleSearchService.Api
```

Swagger (desarrollo): `https://localhost:7117/swagger` (según `launchSettings.json`).

## Próximos pasos previstos

- Persistencia MySQL (vehículos, reservas) y MongoDB (catálogo de mercados y tipos).
- Implementación de `ISearchVehiclesQueryHandler` e `ICreateReservationCommandHandler`.
- Publicación in-memory de `VehicleReservedEvent` tras crear reserva.

## Licencia

Uso educativo / prueba técnica.

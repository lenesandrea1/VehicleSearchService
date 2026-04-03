# VehicleSearchService

## Descripción

Web API para el reto **Outlet Rental Cars**: los clientes **buscan** vehículos disponibles según localidad de recogida y devolución y un rango en **UTC**, y **crean reservas** con reglas de negocio explícitas. La solución prioriza **coherencia documentación–código**, **decisiones justificadas** y **pruebas de integración reales** (Docker / Testcontainers).

## Por qué esta arquitectura y esta tecnología

| Elección | Motivo |
|----------|--------|
| **MySQL** (EF Core) | Información **transaccional** con **integridad referencial** (localidades, flota, reservas). Encaja con consistencia fuerte y modelo relacional claro. |
| **MongoDB** | **Catálogo** (mercados, tipos de vehículo): esquema **evolutivo**, bajo acoplamiento con el agregado transaccional; solo lectura desde la API de búsqueda. |
| **Capas (Clean Architecture)** | **Dominio** sin dependencias de infraestructura; **aplicación** define puertos; **infraestructura** implementa adaptadores. Las reglas de negocio permanecen testeables y estables al cambiar persistencia. |
| **CQRS ligero** | **Query** (`SearchVehiclesQueryHandler`) vs **command** (`CreateReservationCommandHandler`) separan lectura y escritura en casos de uso distintos, sin bus global. |

## Estructura del código

| Proyecto | Rol |
|----------|-----|
| `VehicleSearchService.Domain` | Entidades, reglas, eventos de dominio |
| `VehicleSearchService.Application` | Puertos, handlers, validación de ventana temporal (`RentalTimeWindowGuard`) |
| `VehicleSearchService.Infrastructure` | MySQL, MongoDB, publicador in-memory de eventos |
| `VehicleSearchService.Api` | HTTP, Swagger, ProblemDetails |
| `VehicleSearchService.Tests.Unit` | Dominio, guard de fechas, handlers con dobles |
| `VehicleSearchService.Tests.Integration` | **Testcontainers** (MySQL + Mongo) + `GET /api/vehicles/search` |

## Datos de seed (escenario demo)

Tras migrar con base vacía se cargan **identificadores estables** (`KnownIds`). Resumen útil para Swagger o curl:

| Id (nombre en código) | Qué demuestra |
|------------------------|-----------------|
| `VehicleEconomy`, `VehicleSuv` | Madrid **EU-ES**, estado **Disponible**, aptos en búsqueda fuera del bloqueo. |
| `VehicleWrongMarket` | Madrid pero solo habilitado para **EU-FR** → **excluido** en recogida Madrid (mercado ES). |
| `VehicleOutOfService` | Madrid **EU-ES** pero estado **No disponible** → **excluido** del listado candidato. |
| `VehicleParis` | París **EU-FR**, disponible → aparece solo en búsqueda con recogida en París. |
| `ReservationSample` | Reserva **confirmada** sobre el economy **10–20 jun 2026 UTC** → **solapa** y excluye ese vehículo en ese rango. |
| `LocationParis` | Segundo **mercado** (`EU-FR`) frente a Madrid/Barcelona (`EU-ES`). |

**Mongo:** mercados `EU-ES` / `EU-FR` y tipos `vt-economy`, `vt-suv`, `vt-compact`, `vt-van`, alineados con el seed relacional.

Si tu MySQL ya tenía datos de una versión anterior del seed, elimina la base o el volumen de Docker y vuelve a migrar para cargar este escenario.

## Reglas de negocio (API y dominio)

- Ventana **semiabierta** `[pickup, return)` en **UTC**; entradas se normalizan a UTC en la API.
- **No** recogida en el pasado (respecto a `TimeProvider`, en producción reloj del sistema).
- **Devolución** estrictamente **posterior** a la recogida.
- Búsqueda: vehículo en **estación de recogida**, **habilitado para el mercado** de esa localidad, **disponible**, sin **reserva activa** (pendiente/confirmada) que solape el rango.

Errores **400** con **ProblemDetails** (`title` / `detail`) si el rango temporal es inválido (búsqueda y reserva).

## Eventos de dominio (reserva creada)

Tras persistir la reserva se publica `VehicleReservedEvent` vía `IDomainEventPublisher` (**in-memory**). Los handlers se ejecutan en proceso; si un handler **lanza**, el error se **registra** y **no revierte** la reserva ya guardada (decisión explícita de **resiliencia** frente a efectos secundarios como logging).

## Contrato HTTP y códigos de error (ProblemDetails)

| Situación | Código | Ejemplo `title` (búsqueda / reserva) |
|-----------|--------|--------------------------------------|
| Cuerpo o query inválido (model binding) | 400 | Validación (`ValidationProblemDetails`) |
| Recogida en pasado o `return <= pickup` | 400 | `Invalid rental period.` |
| Vehículo / localidad inexistente en reserva | 404 | `Vehicle not found.` / `Pickup location not found.` / `Return location not found.` |
| Vehículo no alquilable (mercado, fechas, solape) | 409 | `Vehicle not available.` |
| Periodo de reserva inválido en dominio | 400 | `Invalid reservation period.` |

## Pruebas de integración (`VehiclesSearchEndpointTests`)

Con **MySQL + Mongo** reales en contenedores (no es solo smoke del host), se comprueba el **endpoint de búsqueda**:

| Test (nombre aproximado) | Cobertura |
|--------------------------|-----------|
| Retorno de vehículos disponibles + etiquetas de catálogo | 200, 2 ítems economy/suv, mercado España. |
| Excluye economy con solape a reserva de ejemplo | Solo SUV en junio 2026. |
| Excluye `VehicleWrongMarket` y `VehicleOutOfService` | No aparecen en resultados Madrid mayo. |
| Búsqueda en París | Un ítem, mercado Francia. |
| Recogida inexistente | Lista vacía. |
| Fechas inválidas | 400 ProblemDetails. |

El smoke del host (`ApiHostFixture`) sigue **sin** bases ni Mongo para arranque mínimo.

## Puesta en marcha rápida (revisor / local)

```bash
docker compose up -d
dotnet run --project src/VehicleSearchService.Api
```

Abre **Swagger** en la URL HTTPS o HTTP de `launchSettings.json` (`/swagger`). Requiere **.NET 8** y Docker para el compose. Los tests de integración necesitan **Docker** para Testcontainers.

Cadena MySQL por defecto en `appsettings.json`: `root` / `local`, base `vehiclesearch`. Mongo: `mongodb://localhost:27017`, base `vehiclesearch_catalog` (sección `Catalog`).

### Windows sin Docker

Configura `ConnectionStrings:DefaultConnection` en `appsettings.Development.json` o **user-secrets**; levanta MySQL y Mongo manualmente o desactiva catálogo con `Catalog:Enabled=false`.

## Ejemplos HTTP (valores del seed)

Sustituye el host (`https://localhost:7182` o el que use tu perfil).

**Búsqueda Madrid (mayo 2026, fuera del bloqueo de la reserva):**

```http
GET /api/vehicles/search?pickupLocationId=a0000001-0000-0000-0000-000000000001&returnLocationId=a0000001-0000-0000-0000-000000000002&pickupAtUtc=2026-05-01T10:00:00Z&returnAtUtc=2026-05-05T10:00:00Z
```

Respuesta 200 esperada: `items` con dos entradas (economy y suv), `pickupMarketId` `EU-ES`, `pickupMarketDisplayName` acorde al catálogo.

**Búsqueda con rango inválido:**

```http
GET /api/vehicles/search?pickupLocationId=a0000001-0000-0000-0000-000000000001&returnLocationId=a0000001-0000-0000-0000-000000000002&pickupAtUtc=2026-06-05T10:00:00Z&returnAtUtc=2026-06-05T10:00:00Z
```

400 con cuerpo ProblemDetails (`title`: invalid rental period).

**Crear reserva (JSON):**

```http
POST /api/reservations
Content-Type: application/json

{
  "vehicleId": "b0000001-0000-0000-0000-000000000002",
  "pickupLocationId": "a0000001-0000-0000-0000-000000000001",
  "returnLocationId": "a0000001-0000-0000-0000-000000000002",
  "pickupAtUtc": "2027-01-10T10:00:00Z",
  "returnAtUtc": "2027-01-15T10:00:00Z"
}
```

201 con `reservationId` si el vehículo está disponible en ese rango; **409** si hay conflicto; **404** si ids no existen (ajusta fechas al futuro respecto al reloj del servidor).

## Compilar y probar

```bash
dotnet build
dotnet test
```

**CI:** workflow en `.github/workflows`; `dotnet test` incluye integración con contenedores en Linux con Docker.

## Migraciones EF

```bash
dotnet ef migrations add Nombre --project src/VehicleSearchService.Infrastructure --startup-project src/VehicleSearchService.Api --output-dir Persistence/Migrations
```

## Licencia

Uso educativo / prueba técnica.

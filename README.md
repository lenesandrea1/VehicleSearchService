# VehicleSearchService

## Descripción

Web API de búsqueda y reserva de vehículos para el reto técnico **Outlet Rental Cars**. Expone una **query** HTTP para buscar vehículos disponibles (localidad de recogida y devolución, ventana en UTC) y un **command** para crear reservas, aplicando reglas de disponibilidad por estación, mercado, estado del vehículo y solapamiento con reservas activas. El dominio está aislado en **Clean Architecture**; los datos transaccionales viven en **MySQL** y el catálogo descriptivo en **MongoDB**.

## Estructura

| Proyecto | Rol |
|----------|-----|
| `VehicleSearchService.Domain` | Entidades, reglas de negocio, eventos de dominio |
| `VehicleSearchService.Application` | Puertos (persistencia, mensajería), queries/commands, handlers |
| `VehicleSearchService.Infrastructure` | EF Core + MySQL, MongoDB para catálogo, publicador in-memory de eventos |
| `VehicleSearchService.Api` | Host HTTP, Swagger, migraciones al arranque (configurable) |
| `VehicleSearchService.Tests.Unit` | Especificaciones del dominio |
| `VehicleSearchService.Tests.Integration` | Pruebas con **Testcontainers** (MySQL + Mongo) para el endpoint de búsqueda; smoke del host sin contenedores |

## Decisiones técnicas

- **Clean Architecture**: el dominio no referencia infraestructura; la aplicación define puertos (`ILocationReadRepository`, `ICatalogReader`, etc.) y los adaptadores viven en Infrastructure.
- **CQRS ligero**: la búsqueda es una **query** con handler dedicado; la reserva es un **command** con otro handler. No hay bus de mensajes global: los eventos de dominio se publican por un contrato (`IDomainEventPublisher`) con una implementación **in-memory** y un manejador que registra en log, suficiente para el alcance del reto.
- **MySQL + EF Core** para datos transaccionales (localidades, vehículos, reservas) con **seed** idempotente y migraciones al arranque opcionales vía configuración.
- **MongoDB** para catálogo de solo lectura (mercados, tipos de vehículo), alineado con `MarketId` y `VehicleTypeCatalogId` del modelo. Si se desactiva el catálogo (`Catalog:Enabled=false`), la búsqueda sigue siendo correcta pero sin etiquetas legibles.
- **Periodos en UTC** con intervalo semiabierto `[inicio, fin)` para coincidir con solapamientos de reservas y evitar ambigüedades en límites. La aplicación valida además que la recogida no sea pasada respecto a `TimeProvider` (en producción el reloj del sistema), lo que permite fijar el tiempo en tests.
- **Pruebas**: reglas de negocio cubiertas en **unitarias** (dominio + handlers con dobles); **integración** del endpoint `GET /api/vehicles/search` contra **contenedores reales** (Testcontainers) para cumplir el enunciado sin depender de MySQL/Mongo instalados en la máquina del desarrollador. Un test ligero del host sigue desactivando migraciones y catálogo para arranque rápido.
- **Errores de API**: respuestas `409` / `400` / `404` en reservas usan **ProblemDetails** (RFC 7807) en lugar de DTOs ad hoc.
- **CI**: GitHub Actions ejecuta `dotnet test`; los tests de integración con Testcontainers requieren **Docker** en el ejecutor (`ubuntu-latest` lo incluye).

## Convenciones de dominio

- Periodos de alquiler en **UTC**, intervalo semiabierto `[inicio, fin)`.
- La **localidad de devolución** puede diferir de la de recogida; la disponibilidad en búsqueda se evalúa respecto a la **recogida** y al **mercado** de esa localidad.
- Las reservas **pendientes** o **confirmadas** bloquean solapamientos; **canceladas** y **completadas** no.
- **Ventana temporal en API (búsqueda y reserva):** la recogida no puede ser anterior a “ahora” (UTC) y la devolución debe ser **estrictamente posterior** a la recogida. Si no se cumple, la API responde **400** con ProblemDetails.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (o superior con `rollForward` en `global.json`).
- **MySQL 8** (local o Docker).
- **MongoDB** (local o Docker) si dejas `Catalog:Enabled` en `true` (valor por defecto).

## Base de datos con Docker

```bash
docker compose up -d
```

Suben **MongoDB 7** (`27017`) y **MySQL 8** (`3306`). Cadena MySQL por defecto: usuario `root`, contraseña `local`, base `vehiclesearch`. El catálogo usa `mongodb://localhost:27017` y base `vehiclesearch_catalog` (`appsettings.json` → sección `Catalog`).

### Catálogo en MongoDB

Colecciones `markets` y `vehicle_types`: la API rellena datos mínimos al arranque si están vacías (`Catalog:SeedOnStartup`), coherentes con el seed relacional (`EU-ES`, `vt-economy`, `vt-suv`). La búsqueda devuelve nombres legibles (`pickupMarketDisplayName`, `vehicleTypeDisplayName` por ítem).

- Sin Mongo (solo MySQL): pon `Catalog:Enabled` en `false`; la búsqueda sigue funcionando pero sin etiquetas del catálogo.
- El **smoke** del `WebApplicationFactory` fuerza `Catalog:Enabled=false` para no requerir Mongo. Los tests del endpoint de búsqueda usan contenedor Mongo dedicado.

### MySQL instalado en Windows (sin Docker)

Con `dotnet run`, el entorno suele ser **Development**. Entonces se cargan `appsettings.json` y **`appsettings.Development.json`**; este último **reemplaza** la cadena `DefaultConnection`.

- Si ves **`Access denied for user 'root'@'localhost'`**, tu contraseña no es `local`. Opciones:
  1. Edita `src/VehicleSearchService.Api/appsettings.Development.json` y pon en `Password=` la contraseña real de `root` (o el usuario que uses). Si `root` no tiene contraseña, deja `Password=;` como está.
  2. O define secretos de usuario (no se commitean):
     ```bash
     cd src/VehicleSearchService.Api
     dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=vehiclesearch;User=root;Password=AQUI_TU_CONTRASEÑA;"
     ```
- Crea la base **`vehiclesearch`** en MySQL si no existe (o el motor la creará al migrar según permisos).

## Compilar, migrar y ejecutar

```bash
dotnet build
dotnet test
dotnet run --project src/VehicleSearchService.Api
```

Los tests de integración del endpoint de búsqueda levantan **MySQL 8** y **MongoDB 7** vía Testcontainers: hace falta **Docker** en ejecución (local o en CI).

Al arranque, si `RunMigrations` es `true`, se aplican migraciones EF y se ejecuta el **seed** (solo si la base está vacía).

**Swagger** (desarrollo): URL HTTPS del `launchSettings.json` + `/swagger`.

### Endpoints principales

- `GET /api/vehicles/search?pickupLocationId=&returnLocationId=&pickupAtUtc=&returnAtUtc=` (ISO 8601 recomendado). La respuesta incluye `items` y, si el catálogo está activo, `pickupMarketId`, `pickupMarketDisplayName` y por vehículo `vehicleTypeDisplayName`.
- `POST /api/reservations` con cuerpo JSON (`vehicleId`, `pickupLocationId`, `returnLocationId`, `pickupAtUtc`, `returnAtUtc`).

### Datos de ejemplo (seed)

Identificadores estables en `KnownIds` (Infrastructure): Madrid, Barcelona, dos vehículos en Madrid y una reserva de ejemplo que bloquea el vehículo “economy” en un rango de fechas (útil para probar conflictos).

## Migraciones EF (CLI)

```bash
dotnet ef migrations add Nombre --project src/VehicleSearchService.Infrastructure --startup-project src/VehicleSearchService.Api --output-dir Persistence/Migrations
```

## Licencia

Uso educativo / prueba técnica.

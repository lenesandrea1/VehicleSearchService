# VehicleSearchService

Web API de búsqueda y reserva de vehículos para el reto técnico **Outlet Rental Cars**, con **Clean Architecture**, dominio con comportamiento explícito y contratos de aplicación separados de la infraestructura.

## Estructura

| Proyecto | Rol |
|----------|-----|
| `VehicleSearchService.Domain` | Entidades, reglas de negocio, eventos de dominio |
| `VehicleSearchService.Application` | Puertos (persistencia, mensajería), queries/commands, handlers |
| `VehicleSearchService.Infrastructure` | EF Core + MySQL, repositorios, publicador in-memory de eventos |
| `VehicleSearchService.Api` | Host HTTP, Swagger, migraciones al arranque (configurable) |
| `VehicleSearchService.Tests.Unit` | Especificaciones del dominio |
| `VehicleSearchService.Tests.Integration` | Host de prueba sin migraciones |

## Convenciones de dominio

- Periodos de alquiler en **UTC**, intervalo semiabierto `[inicio, fin)`.
- La **localidad de devolución** puede diferir de la de recogida; la disponibilidad en búsqueda se evalúa respecto a la **recogida** y al **mercado** de esa localidad.
- Las reservas **pendientes** o **confirmadas** bloquean solapamientos; **canceladas** y **completadas** no.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (o superior con `rollForward` en `global.json`).
- **MySQL 8** (local o Docker).

## Base de datos con Docker

```bash
docker compose up -d
```

Cadena de conexión por defecto en `appsettings.json`: usuario `root`, contraseña `local`, base `vehiclesearch`, puerto `3306`.

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

Al arranque, si `RunMigrations` es `true`, se aplican migraciones EF y se ejecuta el **seed** (solo si la base está vacía).

**Swagger** (desarrollo): URL HTTPS del `launchSettings.json` + `/swagger`.

### Endpoints principales

- `GET /api/vehicles/search?pickupLocationId=&returnLocationId=&pickupAtUtc=&returnAtUtc=` (ISO 8601 recomendado).
- `POST /api/reservations` con cuerpo JSON (`vehicleId`, `pickupLocationId`, `returnLocationId`, `pickupAtUtc`, `returnAtUtc`).

### Datos de ejemplo (seed)

Identificadores estables en `KnownIds` (Infrastructure): Madrid, Barcelona, dos vehículos en Madrid y una reserva de ejemplo que bloquea el vehículo “economy” en un rango de fechas (útil para probar conflictos).

## Migraciones EF (CLI)

```bash
dotnet ef migrations add Nombre --project src/VehicleSearchService.Infrastructure --startup-project src/VehicleSearchService.Api --output-dir Persistence/Migrations
```

## Pendiente respecto al enunciado

- **MongoDB** para catálogo (mercados / tipos de vehículo): el modelo ya expone `MarketId` y `VehicleTypeCatalogId`; falta integrar lectura desde Mongo.

## Licencia

Uso educativo / prueba técnica.

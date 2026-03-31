# Agendamiento — Backend Project Prompt

> Use this document to onboard an AI assistant into the current state of the project, its architecture, decisions made, and what remains to be done.

---

## What Is This Project?

A multi-tenant SaaS **appointment scheduling API** built with ASP.NET Core (.NET 10). It allows businesses (tenants) to expose a public booking page where clients can schedule appointments with professionals. Admins manage their business configuration (professionals, services, availability) through a protected API.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 Web API |
| Database | PostgreSQL (via EF Core 10 + Npgsql) |
| ORM | Entity Framework Core 10 with snake_case naming |
| Auth | JWT Bearer (BCrypt for password hashing) |
| Background Jobs | Hangfire + Hangfire.PostgreSql |
| WhatsApp Notifications | Twilio |
| API Docs | Swagger (Swashbuckle) |
| Target Frontend | Angular (CORS configured for `http://localhost:4200`) |

---

## Solution Structure

```
Agendamiento/
├── Agendamiento.slnx
├── Agendamiento.Api/           # Web API layer (controllers, Program.cs)
├── Agendamiento.Service/       # Business logic (services + Hangfire job)
├── Agendamiento.Data/          # EF Core (DbContext + Entities)
└── Agendamiento.ViewModel/     # DTOs / request+response models
```

---

## Domain Entities

### Tenant
The business/company using the platform. Identified publicly by a unique `Slug`.
- `Id`, `Nombre`, `Slug` (unique), `LogoUrl`, `ColorPrimario`, `FotoPortada`
- `Telefono`, `Email`, `Descripcion`, `Direccion`, `Instagram`
- `Activo`, `CreadoEn`
- Nav: `Usuarios`, `Profesionales`, `Servicios`, `Reservas`

### Usuario
Staff/admin account belonging to a Tenant.
- `Id`, `TenantId`, `Nombre`, `Email`, `PasswordHash`, `Rol` (default: "Admin"), `Activo`

### Profesional
A service provider within a Tenant (e.g., a hairdresser, doctor).
- `Id`, `TenantId`, `Nombre`, `Descripcion`, `FotoUrl`, `Activo`
- Nav: `Disponibilidades`, `Reservas`

### Servicio
A service offered by a Tenant.
- `Id`, `TenantId`, `Nombre`, `DuracionMinutos`, `Precio`, `Activo`

### Disponibilidad
A professional's weekly recurring availability window.
- `Id`, `ProfesionalId`, `DiaSemana` (0=Sunday … 6=Saturday), `HoraInicio`, `HoraFin`, `Activo`

### Reserva
A client booking.
- `Id`, `TenantId`, `ProfesionalId`, `ServicioId`
- `ClienteNombre`, `ClienteTelefono`, `ClienteEmail`
- `FechaHora` (DateTimeOffset), `DuracionMinutos`
- `Estado` (default: "Pendiente" → Confirmada / Cancelada / Completada)
- `RecordatorioEstado` (default: "Pendiente" → Enviado / Omitido / Error)
- `CreadoEn`

---

## API Endpoints

### Public — No Auth Required

| Method | Route | Description |
|---|---|---|
| GET | `/api/tenants` | List active tenants (minimal info) |
| GET | `/api/booking/{slug}` | Get public tenant info by slug |
| GET | `/api/booking/{slug}/servicios` | List active services |
| GET | `/api/booking/{slug}/profesionales` | List active professionals |
| GET | `/api/booking/{slug}/slots?profesionalId=X&servicioId=Y&fecha=YYYY-MM-DD` | Get available time slots |
| POST | `/api/booking/{slug}/reservar` | Create a reservation |
| POST | `/api/auth/register` | Register a new Tenant + Admin user (returns JWT) |
| POST | `/api/auth/login` | Login (returns JWT) |

### Admin — JWT Required (`[Authorize]`)

#### Config (`/api/config`)
| Method | Route | Description |
|---|---|---|
| POST | `/api/config/servicios` | Create service |
| GET | `/api/config/servicios` | List services for tenant |
| PUT | `/api/config/servicios/{id}` | Update service |
| DELETE | `/api/config/servicios/{id}` | Soft-delete service |
| POST | `/api/config/profesionales` | Create professional |
| GET | `/api/config/profesionales` | List professionals |
| PUT | `/api/config/profesionales/{id}` | Update professional |
| DELETE | `/api/config/profesionales/{id}` | Soft-delete professional |
| POST | `/api/config/disponibilidad` | Create availability block |
| GET | `/api/config/disponibilidad/{profesionalId}` | List availability for professional |
| DELETE | `/api/config/disponibilidad/{id}` | Soft-delete availability |
| GET | `/api/config/negocio` | Get business info |
| PUT | `/api/config/negocio` | Update business info |

#### Reservas Admin (`/api/admin`)
| Method | Route | Description |
|---|---|---|
| GET | `/api/admin/reservas?fecha=YYYY-MM-DD` | List reservations (optional date filter) |
| PATCH | `/api/admin/reservas/{id}/estado` | Change reservation state |

---

## Key Business Logic

### Slot Calculation (`BookingService.GetSlotsDisponiblesAsync`)
1. Finds the professional's `Disponibilidad` for the requested date's day of week.
2. Generates candidate slots every N minutes (where N = service duration).
3. Filters out slots that overlap with existing `Reservas` (Estado ≠ "Cancelada").
4. Returns remaining slots as `List<TimeOnly>`.

### Registration Flow (`AuthService.RegistrarAsync`)
1. Creates a new `Tenant` record.
2. Creates a `Usuario` (Admin) linked to that Tenant.
3. Password hashed with BCrypt.
4. Returns JWT with `TenantId` and `Email` claims.

### Reminder Job (`RecordatorioJob.EnviarRecordatorio`)
- Fires 24 hours before the appointment via Hangfire.
- Sends a WhatsApp message to `ClienteTelefono` via Twilio.
- Updates `RecordatorioEstado` → "Enviado", "Omitido" (too close or cancelled), or "Error".

---

## Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=agendamiento_dev;Username=postgres;Password=..."
  },
  "Jwt": {
    "Key": "clave-super-secreta-cambiar-en-produccion-min32chars",
    "Issuer": "agendamiento-api",
    "Audience": "agendamiento-client"
  },
  "Twilio": {
    "AccountSid": "...",
    "AuthToken": "...",
    "FromNumber": "whatsapp:+14155238886"
  }
}
```

- CORS allows `http://localhost:4200`
- Hangfire dashboard at `/hangfire` (dev only)
- JWT tokens expire in 8 hours

---

## Conventions & Patterns

- **Multi-tenancy:** `TenantId` is extracted from JWT claims in each protected service. All queries filter by `TenantId`.
- **Soft deletes:** Use `Activo = false` flag — no hard deletes in normal flows.
- **DB naming:** All tables and columns use `snake_case` (via `EFCore.NamingConventions`).
- **DI:** Services registered as Scoped. `AppDbContext` is Scoped.
- **No Repository pattern:** Services directly use `AppDbContext`.
- **DTOs:** All API inputs/outputs go through ViewModels in `Agendamiento.ViewModel`. Entities are never exposed directly.

---

## What Is Complete

- [x] Multi-tenant auth (register + login with JWT)
- [x] Public booking flow (tenant page, services, professionals, slots, create reservation)
- [x] Admin config (CRUD for services, professionals, availability, business info)
- [x] Admin reservations list + state change
- [x] Hangfire WhatsApp reminder job via Twilio
- [x] PostgreSQL schema via EF Core

---

## Known Gaps / What Could Be Added

These were identified but **not yet implemented**:

1. **Password reset / change** — no endpoint exists
2. **Multi-user management** — cannot add additional staff users per tenant; no list/delete user endpoints
3. **Input validation** — no `[Required]`/`[MaxLength]` data annotations; `TimeOnly.Parse()` can throw on bad input
4. **Global error handling** — no middleware or `IExceptionHandler`; services don't catch exceptions
5. **Reservation reschedule** — state machine only allows status changes, no date/time update
6. **Availability update** — only create + soft-delete, no PUT for existing slots
7. **Slot collision edge cases** — availability end time boundary not strictly validated
8. **Security hardening** — credentials in `appsettings.json` (should move to User Secrets / env vars); no rate limiting
9. **Testing** — no test project exists
10. **Logging** — `ILogger` injected but minimally used outside `RecordatorioJob`
11. **Pagination** — no pagination on list endpoints
12. **Email notifications** — only WhatsApp via Twilio; no email fallback

---

## File Map (Source Only)

```
Agendamiento.Api/
  Program.cs
  Controllers/
    AuthController.cs
    BookingController.cs
    ConfigController.cs
    ReservasAdminController.cs
    TenantsController.cs
  appsettings.json
  appsettings.Development.json

Agendamiento.Service/
  AuthService.cs
  BookingService.cs
  ConfigService.cs
  ReservaAdminService.cs
  RecordatorioJob.cs

Agendamiento.Data/
  AppDbContext.cs
  Entities/
    Tenant.cs
    Usuario.cs
    Profesional.cs
    Servicio.cs
    Disponibilidad.cs
    Reserva.cs

Agendamiento.ViewModel/
  AuthViewModels.cs
  BookingViewModels.cs
  ConfigViewModels.cs
  ReservaAdminViewModels.cs
```

---

## How to Continue Work

When picking up a task in this project:
1. Read the relevant Controller to understand the API contract.
2. Read the corresponding Service for business logic.
3. Read the Entities to understand the data model.
4. Read the ViewModels to understand request/response shapes.
5. Follow existing patterns: no repositories, direct `AppDbContext` usage, TenantId from JWT claims, soft deletes via `Activo`.
6. Do not expose entities directly — always map to/from ViewModels.

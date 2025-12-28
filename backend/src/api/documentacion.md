A continuación te dejo una **guía de arquitectura** para la carpeta `Api` en una solución .NET (Minimal APIs o controladores). Es “README‑style” para que la pegues en tu repo.

---

# Carpeta `Api` — responsabilidades y estructura

## Propósito

Alojar **el borde HTTP** de la aplicación: endpoints, contratos (DTOs), validación, autenticación/autorización, documentación OpenAPI, manejo de errores, telemetría y salud. Debe ser  **delgada** : no contiene lógica de negocio ni acceso a datos (eso vive en `Application`/`Infrastructure`).

## Detalle por carpeta

### `Program.cs`

Compone el **pipeline HTTP** y registra servicios:

* `UseAuthentication()`, `UseAuthorization()`; `[Authorize]` sobre endpoints/policies. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
* **OpenAPI** (`AddOpenApi`, `MapOpenApi`) y **Swagger UI/Scalar solo en `Development`** (reduce exposición). [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession?view=aspnetcore-10.0)
* **Health checks** (`AddHealthChecks`, `MapHealthChecks("/health")`). [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)
* **Logging** (`ILogger` o Serilog) con niveles y no PII. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.ipasswordhasher-1?view=aspnetcore-10.0)

> Minimal APIs usa **route groups** y **WithOpenApi** para describir endpoints. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2279797/persisting-data-through-session)

---

### `Endpoints/` (o `Controllers/`)

Los endpoints  **orquestan** : reciben DTOs, validan, llaman a `Application` y devuelven códigos HTTP apropiados.

* Minimal APIs: agrupa por recurso (`/users`, `/roles`) y documenta con `.WithOpenApi()`. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2279797/persisting-data-through-session)
* Controladores MVC: `[ApiController]` + `[Authorize]` + atributos de respuesta.

**Ejemplo (Minimal API):**

publicstaticclassUsersEndpoints

{

    publicstaticIEndpointRouteBuilderMapUsers(thisIEndpointRouteBuilderroutes)

    {

    vargroup=routes.MapGroup("/users").WithTags("Users");

    group.MapPost("/",CreateUser).Produces(201).WithOpenApi();

    group.MapGet("/{id:int}",GetUser).Produces(200).Produces(404).WithOpenApi();

    // …

    returnroutes;

    }

}

---

### `Contracts/`

Define **DTOs** de entrada/salida. Añade **DataAnnotations** (o FluentValidation) para reglas básicas; en controladores con `[ApiController]` obtienes **400 automático** si el modelo es inválido. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2114933/how-to-validate-security-stamp-on-next-request)

---

### `Validation/`

Validación adicional (p.ej. FluentValidation) o validación de negocio superficial para rechazar rápido.

> En web APIs, mantener las reglas de formato/tamaño aquí y las reglas de negocio profundas en `Application`. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2114933/how-to-validate-security-stamp-on-next-request)

---

### `Auth/`

* **Policies** (claims/roles) con `AddAuthorization`, `RequireClaim`, `RequireRole`, `RequireAssertion`. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/390717/how-to-combine-one-to-many-relationship-and-many-t)
* **Roles** : `[Authorize(Roles="Administrador,Soporte")]` donde aplique. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew)
* **Tokens/cookies** : configura esquemas en `Program.cs` y usa claims para autorización. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)

---

### `Middleware/`

Cross‑cutting:

* **Global exception handler** → 500 consistente + problema (RFC7807) en JSON.
* **Request/Response logging** y **Correlation ID** (cabecera `X-Correlation-ID`).
* **Rate limiting** si procede.

> Logging con `ILogger` o Serilog; evita PII y usa niveles adecuados. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.ipasswordhasher-1?view=aspnetcore-10.0)

---

### `OpenApi/`

Configura la generación de documento y UI (Swagger/Scalar).  **Habilitar UI sólo en `Development`** . Documenta respuestas (`Produces`) y esquemas. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.isession?view=aspnetcore-10.0)

---

### `Health/`

Registra sondas de **liveness** y **readiness** (DB, servicios externos) y expone `/health`. Útil para orquestadores y balanceadores. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)

---

### `Versioning/` (opcional)

Si tu API tendrá múltiples versiones, organiza route groups por versión (`/v1`, `/v2`) y etiqueta OpenAPI por versión. Guía general de versionado/semver para paquetes/bibliotecas. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/performance/overview?view=aspnetcore-10.0)

---

### `Mapping/` (opcional)

Perfiles de AutoMapper para mapear entidades ↔ DTOs. Mantén el mapeo simple en la capa `Api`.

---

### `appsettings.json` (sin secretos)

Solo **configuración no sensible** (logging, OpenAPI, flags). **No** metas credenciales: usa **variables de entorno** en dev y **Azure Key Vault** en prod; si despliegas en Azure, considera **Managed Identity** para acceso a recursos sin secretos. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-10.0)

---

## Reglas de diseño de API (resumen)

* **Idempotencia** : `GET/PUT/DELETE` idempotentes; `POST` para crear.
* **Códigos HTTP y contratos** : usa `Results.*`/`Produces()` con OpenAPI. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2279797/persisting-data-through-session)
* **Paginación, filtro, orden** para colecciones grandes (evita payloads enormes). [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/1476624/using-serilog-in-asp-net-core-solution-with-multip)
* **Autorización declarativa** con roles/policies; evita consultas a BD por request para “quién es/qué puede”. Claims en token/cookie + revalidación de security stamp. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/390717/how-to-combine-one-to-many-relationship-and-many-t)
* **Observabilidad** : logs estructurados por request y  **health checks** . [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.ipasswordhasher-1?view=aspnetcore-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)

---

## Qué **debe llevar** hoy tu carpeta `Api` (según tu repo actual)

* `Program.cs` con:
  * Registro de `MexyContext` y servicios.
  * Endpoints `Users`/`Roles` (Minimal APIs) con DTOs y validación.
  * `AddOpenApi` + `MapOpenApi`  **solo en dev** . [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-10.0)
  * `AddHealthChecks()` + `MapHealthChecks("/health")`. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)
  * Logging básico (`AddConsole`).
* `Endpoints/UsersEndpoints.cs` y `Endpoints/RolesEndpoints.cs` para ordenar rutas. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2279797/persisting-data-through-session)
* `Contracts/` con `CreateUserDto`, `UserResponse`, `RoleDto`.
* `Validation/` (si amplías más allá de DataAnnotations). [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/2114933/how-to-validate-security-stamp-on-next-request)
* `Auth/Policies.cs` si empiezas a proteger endpoints por roles/políticas. [[learn.microsoft.com]](https://learn.microsoft.com/en-us/answers/questions/390717/how-to-combine-one-to-many-relationship-and-many-t)
* `OpenApi/OpenApiConfig.cs` (opcional) para centralizar la configuración de tags, descriptions y `Produces`.
* `Health/HealthConfig.cs` (opcional) para chequear DB/externos.
* `appsettings.json` **sin secretos** (mover credenciales a env/Key Vault). [[learn.microsoft.com]](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-10.0), [[learn.microsoft.com]](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-10.0)

---

## Siguiente paso rápido

Si quieres, preparo un **README.md** listo para tu `src/Api/` con esta estructura, más un ejemplo completo de `UsersEndpoints` (Minimal API + OpenAPI + validación) y el registro de health/logging en `Program.cs`. ¿Lo dejo con DataAnnotations o prefieres FluentValidation?

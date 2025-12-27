
using MexyApp.Api.Domain;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Iniciando MexyApp");

var builder = WebApplication.CreateBuilder(args);

// DbContext → PostgreSQL (Supabase pooler 6543)
// Mantén timeouts y reintentos para operación normal (lecturas/escrituras)
builder.Services.AddDbContext<MexyContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsql =>
        {
            npgsql.CommandTimeout(180);
            npgsql.EnableRetryOnFailure(8, TimeSpan.FromSeconds(8), null);
            // No configures MigrationsAssembly si no vas a migrar en runtime
            // npgsql.MigrationsAssembly(typeof(MextyContext).Assembly.GetName().Name);
        }
    )
);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Endpoint de salud de base de datos (opcional)
app.MapGet("/ping-db", async (MexyContext db) =>
{
    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        return Results.Ok(new { ok = true, message = "Conexión a Supabase exitosa" });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Error de conexión", detail: ex.Message, statusCode: 500);
    }
});

// Importante: NO migres ni hagas seed aquí si el esquema ya fue aplicado por SQL
// await app.MigrateAndSeedAsync(...);  // ← ELIMINADO
// using (var scope = app.Services.CreateScope()) { ... db.Database.MigrateAsync(); } // ← ELIMINADO

app.Run();

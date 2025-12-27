// EF Core (Entity Framework Core) es un ORM de .NET

/* ¿Qué es un ORM?

   Es como un traductor entre tu código y la base de datos.

   Tú trabajas con clases (User, Product, etc.) y EF Core se encarga de:

- Convertir tus consultas LINQ en SQL
- Insertar, actualizar y borrar datos
- Mapear filas de la DB a objetos C# y viceversa

   ¿Para qué sirve?

- Evitas escribir SQL manual en cada operación
- Mapeas tablas ↔ clases
- Migraciones: actualizas el esquema desde código
- Funciona con varias DB: SQL Server, PostgreSQL, MySQL, SQLite...

   Conceptos clave:

1) DbContext → Es la “puerta” a la DB, tu sesión de trabajo

   public class AppDbContext : DbContext {

   public DbSet`<User>` Users => Set`<User>`();

   }
2) DbSet`<T>` → Representa una tabla (ej: db.Users = tabla Users)
3) LINQ → SQL

   var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

   // EF genera: SELECT TOP(1) * FROM Users WHERE Email = ...
4) Migraciones → Scripts para crear/actualizar tablas desdecódigo

   Diferenciarápida:

   -EFclásico(EF6):viejo,ligadoa.NETFramework

   -EFCore:moderno,multiplataforma,recomendadohoy

   ¿Porqué apareceprivateUser(){}// EF?

   PorqueEFnecesitacrearobjetosUsercuandoleedelaDB.

   Eseconstructorvacíoesla “puerta” paraEF,perolohacemosprivate

   paraquenadiemáspuedacrearUsersvacíosyromperreglas.

   SinousasEFCore,opciones:

   -Dapper(másmanual,rápido)

   -SQLdirectoconADO.NET

   Paraproyectosnuevos,EFCoreaceleramuchoelCRUDymigraciones.

*/

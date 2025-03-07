**Nota:** Actualmente no se subieron las migraciones, se subirán después. Por el momento, ejecuta los siguientes 
comandos para llenar la base de datos:

1. `dotnet ef migrations add AddAllTables`
2. `dotnet ef database update` *(Aplica los cambios a la base de datos)*
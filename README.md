**Nota:** Actualmente no se subieron las migraciones, se subirán después. Por el momento, ejecuta los siguientes 
comandos para llenar la base de datos:

1. `dotnet ef migrations add AddAllTables`
2. `dotnet ef database update` *(Aplica los cambios a la base de datos)*
3. `dotnet ef migrations remove` * migración sigue fallando, intenta revertirla y volver a generarla *


En tu .env tambien cambia el valor de la propiedad JWT_SECRET, tienes que poner una cadena de texto segura para que funcione (si dejas 'JWT_SECRET' el programa te dira que hubo un error al intentar correr el servidor), usa lastpass para generarlo si asi lo deseas.


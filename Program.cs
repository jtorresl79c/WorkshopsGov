using WorkshopsGov.Configurations;
using WorkshopsGov.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkshopsGov.Models;
using DotNetEnv;
using WorkshopsGov.Seeders;
using Microsoft.AspNetCore.Identity.UI.Services;
using WorkshopsGov.Services; // Asegúrate de importar el namespace
using Microsoft.Extensions.DependencyInjection;

Env.Load(); // Carga las variables desde el archivo .env

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddViteManifest();

// Add services to the container.
string dbUrl = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION") ?? throw new InvalidOperationException("POSTGRESQL_CONNECTION not found.");
var connectionString = dbUrl ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Permite usar vistas normales (MVC) y APIs con [ApiController]
builder.Services.AddControllersWithViews(); // builder.Services.AddMvc();
builder.Services.AddRazorPages();

// Habilita Swagger para documentar y probar las APIs
// Con esto, puedes usar https://localhost:5001/swagger para probar tus APIs y seguir desarrollando la interfaz con
// Razor Views. 🚀
/*
 * builder.Services.AddEndpointsApiExplorer();
 * Registra los endpoints de API en el sistema de documentación de Swagger.
 * Es necesario para que Swagger pueda descubrir los endpoints expuestos por Minimal APIs en .NET 6+.
 * Si solo usas controladores ([ApiController]), puedes omitirlo, pero si usas Minimal APIs, debes incluirlo.
 */
/*
 * builder.Services.AddSwaggerGen();
 * Agrega y configura Swagger en la aplicación
 * Genera la documentación de la API en formato OpenAPI
 * Permite personalizar la documentación con atributos como [ApiExplorerSettings], [Produces], [Consumes], etc.
 */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/////////////// JWT
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET not found.");
var jwtExpiry = Environment.GetEnvironmentVariable("JWT_EXPIRY_TIME_FRAME") ?? throw new InvalidOperationException("JWT_EXPIRY_TIME_FRAME not found.");
builder.Services.Configure<JwtConfig>(options =>
{
    options.Secret = jwtSecret;
    options.ExpiryTimeFrame = TimeSpan.Parse(jwtExpiry);
});

var key = Encoding.ASCII.GetBytes(jwtSecret);
var tokenValidationParameter = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false, // for dev, for production put in true
    ValidateAudience = false, // for dev, for production put in true
    RequireExpirationTime = false,// for dev, for production put in true
    ValidateLifetime = true
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameter;
});

builder.Services.AddSingleton(tokenValidationParameter);


//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

//MANDA UN ERROR AL MOMENTO DE REGISTRARSE HAY QUE VERIFICAR ESTA FUNCIONALIDAD
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // ¡Esto es crucial para las páginas de Identity UI!
//////////////////////////////////////////////////// JWT
// Agrega esto antes de builder.Build();
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();

// Configurar el servicio SPA
builder.Services.AddSpaStaticFiles(configuration => {
    configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/vue-apps"))
    {
        Console.WriteLine($"Solicitando: {context.Request.Path}");
    }
    await next();
});

// Ejecutar seeders manualmente
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        
        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await AspNetRolesSeeder.SeedAsync(roleManager);

        BrandSeeder.Seed(context);
        SectorSeeder.Seed(context);
        ExternalWorkshopSeeder.Seed(context);
        VehicleStatusSeeder.Seed(context);
        VehicleTypeSeeder.Seed(context);
        VehicleFailureSeeder.Seed(context);
        InspectionPartSeeder.Seed(context);
        InspectionStatusSeeder.Seed(context);
        InspectionServiceSeeder.Seed(context);
        VehicleModelSeeder.Seed(context);
        DepartmentSeeder.Seed(context);
        await ApplicationUserSeeder.SeedAsync(userManager);
        ExternalWorkshopBranchSeeder.Seed(context);
        VehicleSeeder.Seed(context);
        InspectionSeeder.Seed(context);
        InspectionVehicleFailureSeeder.Seed(context);
        FileTypeSeeder.Seed(context);
        FileSeeder.Seed(context);
        InspectionFileSeeder.Seed(context);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Si la aplicación se está ejecutando en un entorno de desarrollo, se utiliza el MigrationsEndPoint, que permite aplicar migraciones de base de datos
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Para el SPA
}
else
{
    /*
     * Si la aplicación se está ejecutando en un entorno de producción:
     * UseExceptionHandler("/Home/Error"): Redirige las excepciones a una página de error personalizada.
     * UseHsts(): Habilita HSTS (HTTP Strict Transport Security), que es una política de seguridad web que ayuda a proteger los sitios web contra ataques de suplantación de identidad y secuestro de cookies.
     */
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Fuerza a la aplicación a redirigir todas las solicitudes HTTP a HTTPS para asegurar las conexiones.

app.UseCors("AllowViteDevServer"); // Agrega esto antes de UseRouting()

// // Middleware esenciales
app.UseStaticFiles();
app.UseSpaStaticFiles(); // Para el SPA vue
app.UseRouting();

// Autentificacion
app.UseAuthentication(); ////// JWT
app.UseAuthorization();

app.MapControllers(); // Activa los endpoints de API

//Redirigir Login como pagina inicial
app.Use(async (context, next) =>
{
    var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

    if (!isAuthenticated && context.Request.Path == "/")
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    await next();
});

app.MapControllerRoute( // Configura las rutas MVC (HomeController, etc.).
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
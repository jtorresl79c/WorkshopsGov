using WorkshopsGov.Configurations;
using WorkshopsGov.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WorkshopsGov.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("PostgresqlConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Permite usar vistas normales (MVC) y APIs con [ApiController]
builder.Services.AddControllersWithViews(); // builder.Services.AddMvc();

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
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
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

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
//////////////////////////////////////////////////// JWT

var app = builder.Build();

// Ejecutar seeders manualmente
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.SeedData();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Si la aplicación se está ejecutando en un entorno de desarrollo, se utiliza el MigrationsEndPoint, que permite aplicar migraciones de base de datos
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
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

// // Middleware esenciales
app.UseStaticFiles();
app.UseRouting();

// Autentificacion
app.UseAuthentication(); ////// JWT
app.UseAuthorization();

app.MapControllers(); // Activa los endpoints de API
app.MapControllerRoute( // Configura las rutas MVC (HomeController, etc.).
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
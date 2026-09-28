using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Services.Interfaces;
using Transportes_Orellana.Services.Implementations;
using QuestPDF.Infrastructure;

// Configuración para permitir fechas sin zona horaria en PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Rutas para login y accesos denegados
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICalculoUtilidadesService, CalculoUtilidadesService>();
builder.Services.AddScoped<IReporteUtilidadesPdfService, ReporteUtilidadesPdfService>();
builder.Services.AddScoped<IGastoFleteService, GastoFleteService>();
builder.Services.AddScoped<IUsuarioService, UserService>();
builder.Services.AddScoped<IMotoristaService, MotoristaService>();
builder.Services.AddScoped<IUnidadTransporteService, UnidadTransporteService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Migraciones y seeding automático para nube o local
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // Aplicar las migraciones de EF Core
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        // Inicializar roles y el primer administrador
        await SeedSecurityDataAsync(services, builder.Configuration);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error al ejecutar las migraciones o el Seeding en la base de datos.");
        // Si falla la DB al arrancar, registrar en logs
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();

// Método de seeding
async Task SeedSecurityDataAsync(IServiceProvider serviceProvider, IConfiguration configuration)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Roles del sistema
    string[] roles = { "Admin", "Motorista" };
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }

    // Obtener credenciales del primer Admin desde Variables de Entorno o appsettings
    string adminEmail = configuration["AdminSeed:Email"] ?? "admin@transportesorellana.com";
    string adminPassword = configuration["AdminSeed:Password"] ?? "Admin!Transportes#Orellana_2026";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(admin, adminPassword);
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
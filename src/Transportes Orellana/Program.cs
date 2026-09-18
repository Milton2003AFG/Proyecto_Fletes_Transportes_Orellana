using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transportes_Orellana.Data;
using Transportes_Orellana.Services.Interfaces;
using Transportes_Orellana.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

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
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Crear roles y admin por defecto al arrancar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedSecurityDataAsync(services);
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

// Método para sembrar Roles y usuario inicial
async Task SeedSecurityDataAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "Motorista" };
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }

    // Crear Admin por defecto si no existe
    string adminEmail = "admin@transportesorellana.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var resultado = await userManager.CreateAsync(admin, "Admin!Transportes#Orellana_2026");
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    // Motorista de prueba
    string motoristaEmail = "motorista@transportesorellana.com";
    var motoristaUser = await userManager.FindByEmailAsync(motoristaEmail);
    if (motoristaUser == null)
    {
        var motorista = new IdentityUser { UserName = motoristaEmail, Email = motoristaEmail, EmailConfirmed = true };
        var resultado = await userManager.CreateAsync(motorista, "Motorista123*");
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(motorista, "Motorista");
        }
    }
}
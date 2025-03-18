using GestionContableYugen.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GestionContableYugen.Mappings;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("GestionContableDB");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine));

// Configuración de la Autenticación con Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    });


builder.Services.AddAuthorization();

// Configuración de la Sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
});


// Servicios Adicionales
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



var app = builder.Build();

// Configuración de Middleware (Orden Correcto)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads")),
    RequestPath = "/uploads"
});

// Configuración del Flujo de la Aplicación
app.UseSession(); 
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



// Rutas de la Aplicación
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "proveedores",
    pattern: "{controller=Proveedor}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "clientes_api",
    pattern: "Clientes/GetClienteData/{id}",
    defaults: new { controller = "Clientes", action = "GetClienteData" });

app.MapControllerRoute(
    name: "cajachica",
    pattern: "{controller=CajaChica}/{action=Index}/{id?}");

// Middleware para Monitoreo de Peticiones y Sesiones
app.Use(async (context, next) =>
{
    Console.WriteLine($"🔍 Request: {context.Request.Path}");
    Console.WriteLine($"🟢 Sesión Usuario: {context.Session.GetString("Usuario")}");
    Console.WriteLine($"🟢 Sesión UsuarioID: {context.Session.GetInt32("UsuarioID")}");
    await next();
});

app.Run();
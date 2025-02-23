using System.Net;
using LiveChat;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

// Agrega esta línea para habilitar que el servidor escuche en todas las interfaces de red.
//builder.WebHost.UseUrls("https://localhost:7259");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7259);  // Escucha en todas las interfaces de red en el puerto 7259 (https)
    options.ListenAnyIP(5286);  // Escucha en todas las interfaces de red en el puerto 5286 (http)
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Index";  // Redirige al login si no está autenticado
                options.AccessDeniedPath = "/Index";  // Si no tiene permiso
                options.SlidingExpiration = true;  // La cookie expira automáticamente después de un tiempo
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Tiempo de expiración de la cookie
            });



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(); // Para Razor Pages (si estás usando @page)

builder.Services.AddSignalR(); // Esta aplicación usa SignalR

// Creo una nueva instancia de UsuarioRepository para cada solicitud HTTP.
builder.Services.AddScoped<UsuarioRepository>();

builder.Services.AddScoped<ConversacionRepository>();


// Crea una única instancia de MongoDBConnection y la reutiliza en toda la aplicación
builder.Services.AddSingleton<MongoDBConnection>(sp => new MongoDBConnection());

/*La inyección de dependencias (Dependency Injection o DI) es un patrón de diseño que ayuda a gestionar las dependencias entre los diferentes componentes de una aplicación de una manera organizada, flexible
  y fácil de mantener. En lugar de que un componente cree directamente sus dependencias (objetos necesarios para funcionar), estas se proporcionan ("inyectan") desde el exterior.*/

/* Gracias a la inyeccion de dependencias puedo evitar  instanciar manualmente los objetos (como MongoDBConnection o UsuarioRepository) en cada lugar donde los necesites. En lugar de eso:

Registro las dependencias en el contenedor (en tu caso, builder.Services en Program.cs).
ASP.NET Core se encarga automáticamente de:
Crear las instancias cuando son necesarias.
Resolver las dependencias de forma correcta y ordenada.
Elegir la "vida útil" (singleton, scoped, transient) según cómo hayas registrado cada servicio.*/

// Me evito este código:
/*
var conexion = new MongoDBConnection();
var UsuarioRepository = new UsuarioRepository(conexion);*/




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configurar el middleware de autenticación
app.UseAuthentication();
app.UseAuthorization();


//app.UseEndpoints(endpoints => {endpoints.MapHub<ChatHub>("/chat")});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Solo si estás usando Razor Pages

app.MapHub<UsuarioHub>("/usuariohub");
app.MapHub<ConversacionHub>("/conversacionhub");





app.Run();

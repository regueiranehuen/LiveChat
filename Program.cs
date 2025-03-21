using System.Net;
using LiveChat;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5286); // HTTP
    options.ListenAnyIP(7259, listenOptions =>
    {
        listenOptions.UseHttps(); // Activa HTTPS en el puerto 7259
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            { 
                options.SlidingExpiration = true;  // La cookie expira autom�ticamente despu�s de un tiempo
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Tiempo de expiraci�n de la cookie
            });



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(); // Para Razor Pages

builder.Services.AddSignalR(); // Esta aplicaci�n usa SignalR

// Creo una nueva instancia de UsuarioRepository para cada solicitud HTTP.
builder.Services.AddScoped<UsuarioRepository>();

builder.Services.AddScoped<ConversacionRepository>();


// Crea una �nica instancia de MongoDBConnection y la reutiliza en toda la aplicaci�n
builder.Services.AddSingleton<MongoDBConnection>();

builder.Services.AddHttpContextAccessor();


/*La inyecci�n de dependencias (Dependency Injection o DI) es un patr�n de dise�o que ayuda a gestionar las dependencias entre los diferentes componentes de una aplicaci�n de una manera organizada, flexible
  y f�cil de mantener. En lugar de que un componente cree directamente sus dependencias (objetos necesarios para funcionar), estas se proporcionan ("inyectan") desde el exterior.*/

/* Gracias a la inyeccion de dependencias puedo evitar  instanciar manualmente los objetos (como MongoDBConnection o UsuarioRepository) en cada lugar donde los necesites. En lugar de eso:

Registro las dependencias en el contenedor (en tu caso, builder.Services en Program.cs).
ASP.NET Core se encarga autom�ticamente de:
Crear las instancias cuando son necesarias.
Resolver las dependencias de forma correcta y ordenada.
Elegir la "vida �til" (singleton, scoped, transient) seg�n c�mo hayas registrado cada servicio.*/

// Me evito este c�digo:
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

// Configurar el middleware de autenticaci�n
app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Uso razor pages

app.MapHub<UsuarioHub>("/usuariohub");
app.MapHub<ConversacionHub>("/conversacionhub");
app.MapHub<ChatHub>("/chathub");

app.Run();
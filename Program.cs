using System.Net;
using LiveChat;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

// Agrega esta l�nea para habilitar que el servidor escuche en todas las interfaces de red.
//builder.WebHost.UseUrls("https://localhost:7259");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7259);  // Escucha en todas las interfaces de red en el puerto 7259 (https)
    options.ListenAnyIP(5286);  // Escucha en todas las interfaces de red en el puerto 5286 (http)
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(); // Esta aplicaci�n usa SignalR

// Creo una nueva instancia de UsuarioRepository para cada solicitud HTTP.
builder.Services.AddScoped<UsuarioRepository>();



// Crea una �nica instancia de MongoDBConnection y la reutiliza en toda la aplicaci�n
builder.Services.AddSingleton<MongoDBConnection>(sp => new MongoDBConnection());

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

app.UseAuthorization();



//app.UseEndpoints(endpoints => {endpoints.MapHub<ChatHub>("/chat")});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<UsuarioHub>("/usuariohub");

app.MapHub<ChatHub>("/chathub"); // Mapear los hubs creados




app.Run();

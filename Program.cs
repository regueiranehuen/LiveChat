using System.Net;
using LiveChat;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Agrega esta línea para habilitar que el servidor escuche en todas las interfaces de red.
//builder.WebHost.UseUrls("https://localhost:7259");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7259);  // Escucha en todas las interfaces de red en el puerto 7259 (https)
    options.ListenAnyIP(5286);  // Escucha en todas las interfaces de red en el puerto 5286 (http)
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(); // Esta aplicación usa SignalR
builder.Services.AddSingleton<UsuarioRepository>();

builder.Services.AddSingleton<MongoDBConnection>(sp => new MongoDBConnection());




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

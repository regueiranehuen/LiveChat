using LiveChat.Hubs; 

var builder = WebApplication.CreateBuilder(args);

// Agrega esta línea para habilitar que el servidor escuche en todas las interfaces de red.
//builder.WebHost.UseUrls("https://localhost:7259");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(); // Esta aplicación usa SignalR

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

app.MapHub<ChatHub>("/chathub"); // Mapear los hubs creados




app.Run();

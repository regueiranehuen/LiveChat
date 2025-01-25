using Microsoft.AspNetCore.SignalR;


namespace LiveChat
{
    public class ChatHub : Hub
    {

        public async Task SendMessage(string user, string message)
        {
            // Clients.All devuelve todos los clientes conectados


            //Console.WriteLine($"juju mensajito: {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now); // A quien sea que esté conectado, enviarle el método ReceiveMessage

            // Si se quiere enviar un mensaje a un usuario en especifico
            //await Clients.User("userId").SendAsync("ReceiveMessage", user, message);
        }
    }


}



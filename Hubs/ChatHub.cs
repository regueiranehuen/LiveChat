using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;


namespace LiveChat
{
    public class ChatHub : Hub
    {

        private ConversacionRepository _conversacionRepository;

        public ChatHub(ConversacionRepository conversacionRepository)
        {
            _conversacionRepository = conversacionRepository;
        }


        public static ConcurrentDictionary<string, string> Usuarios = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string usuario = Context.User.Identity.Name; // Obtener usuario autenticado
            string connectionId = Context.ConnectionId; // Obtener ID de conexión

            System.Diagnostics.Debug.WriteLine("usuario sigma:" + usuario);
            System.Diagnostics.Debug.WriteLine("connection id sigma:" + connectionId);

            if (!string.IsNullOrEmpty(usuario))
            {
                Usuarios[usuario] = connectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Usuarios.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<Mensaje>> ObtenerMensajesDeConversacion(string idConversacion)
        {
            return await _conversacionRepository.GetMensajesDeConversacion(idConversacion);
        }

        public async Task SendMessage(string user, string message)
        {
            // Clients.All devuelve todos los clientes conectados


            //Console.WriteLine($"juju mensajito: {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now); // A quien sea que esté conectado, enviarle el método ReceiveMessage

            // Si se quiere enviar un mensaje a un usuario en especifico
            //await Clients.User("userId").SendAsync("ReceiveMessage", user, message);
        }


        // Acá habria que hacer que el mensaje recibido x parámetro sea un string, el objeto Mensaje hay que crearlo dentro de la función
        public async Task<Mensaje> EnviarMensaje(string idConversacion,string textoMensaje, string emisor, string destinatario)
        {

            Mensaje mensaje = new Mensaje(idConversacion, emisor, destinatario, textoMensaje, DateTime.Now);

            

            if (Usuarios.TryGetValue(destinatario, out string connectionId))
            {
                

                System.Diagnostics.Debug.WriteLine($"Enviando mensaje a connectionId: {connectionId}");
                await Clients.Client(connectionId).SendAsync("RecibirMensaje", mensaje);
                System.Diagnostics.Debug.WriteLine($"Mensaje enviado a {connectionId}");

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("El cliente no está conectado, pero guardamos igualmente el mensaje en la BD");
            }

            await _conversacionRepository.AgregarMensajeAColeccionMensajes(mensaje);
            await _conversacionRepository.AgregarMensajeAConversacion(mensaje.IdConversacion,mensaje);

            return mensaje;
            


        }

    }


}



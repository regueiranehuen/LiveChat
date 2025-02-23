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
        public async Task EnviarMensaje(string idConversacion,string textoMensaje, string emisor, string destinatario)
        {

            Mensaje mensaje = new Mensaje(idConversacion, emisor, destinatario, textoMensaje, DateTime.Now);

            string connectionId = ConversacionHub.Usuarios.FirstOrDefault(usuario => usuario.Equals(destinatario)).ToString();

            if (connectionId != null)
            {
                // Envía el mensaje al usuario destino
                System.Diagnostics.Debug.WriteLine("El destinatario esta conectado");
                await Clients.Client(connectionId).SendAsync("RecibirMensaje", mensaje);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("El cliente no está conectado, pero guardamos igualmente el mensaje en la BD");
            }

            await _conversacionRepository.AgregarMensajeAColeccionMensajes(mensaje);
            await _conversacionRepository.AgregarMensajeAConversacion(mensaje.IdConversacion,mensaje);
            


        }

    }


}



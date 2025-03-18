using System.Collections.Concurrent;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;


namespace LiveChat
{
    public class ChatHub : Hub
    {
        
        private ConversacionRepository _conversacionRepository;
        private IHubContext<ConversacionHub> _conversacionHubContext;
        public ChatHub(ConversacionRepository conversacionRepository, IHubContext<ConversacionHub> conversacionHubContext)
        {
            _conversacionRepository = conversacionRepository;
            _conversacionHubContext = conversacionHubContext;
        }


        public static ConcurrentDictionary<string, string> Usuarios = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string usuario = Context.User.Identity.Name; // Obtener usuario autenticado
            string connectionId = Context.ConnectionId; // Obtener ID de conexión


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

        public async Task<List<Mensaje>?> ObtenerMensajesDeConversacion(string idConversacion)
        {
            // Evitar que se pasen de vivos modificando codigo JS
            string[] usuarios = idConversacion.Split(',');
            string usuarioAutenticado = Context.User.Identity.Name;

            if (!usuarioAutenticado.Equals(usuarios[0]) && !usuarioAutenticado.Equals(usuarios[1]))
            {
                return null;
            }

            return await _conversacionRepository.GetMensajesDeConversacion(idConversacion);
        }


        public async Task<Mensaje?> EnviarMensaje(string idConversacion,string textoMensaje, string emisor, string destinatario)
        {
            bool usuarioConectado = false;

            // Evitar que se pasen de vivos modificando código JS
            string usuarioAutenticado = Context.User.Identity.Name; // Obtener usuario autenticado

            if (textoMensaje.Equals("") || // No se pueden enviar mensajes vacíos
                _conversacionRepository.ExisteConversacion(emisor, destinatario) == null || 
                !emisor.Equals(usuarioAutenticado) || 
                _conversacionRepository.ObtenerConversacionPorId(idConversacion)==null ||
                (!idConversacion.Equals(_conversacionRepository.CrearIdConversacion(emisor,destinatario)) &&
                !idConversacion.Equals(_conversacionRepository.CrearIdConversacion(destinatario,emisor))))
            {
                return null;
            }

            Mensaje mensaje = new Mensaje(idConversacion, emisor, destinatario, textoMensaje, DateTime.Now);

            // Si el usuario está conectado al ChatHub
            if (Usuarios.TryGetValue(destinatario, out string connectionId))
            {
                usuarioConectado = true;
                await Clients.Client(connectionId).SendAsync("RecibirMensaje", mensaje);
            }
            // Si el usuario está conectado al ConversacionHub (Si está en la pestaña de conversaciones)
            if (ConversacionHub.Usuarios.TryGetValue(destinatario, out string connectionId2))
            {
                usuarioConectado = true;
                await _conversacionHubContext.Clients.Client(connectionId2).SendAsync("RecibirMensaje", mensaje);
            }

            if (!usuarioConectado)
                System.Diagnostics.Debug.WriteLine("El cliente no está conectado a ningun hub, pero guardamos igualmente el mensaje en la BD");

            await _conversacionRepository.AgregarMensajeAColeccionMensajes(mensaje);
            await _conversacionRepository.AgregarMensajeAConversacion(mensaje.IdConversacion,mensaje);

            return mensaje;
        }

    }


}



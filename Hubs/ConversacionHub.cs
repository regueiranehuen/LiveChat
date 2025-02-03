using Microsoft.AspNetCore.SignalR;

namespace LiveChat
{
    public class ConversacionHub : Hub
    {
        private ConversacionRepository _conversacionRepository;

        public ConversacionHub(ConversacionRepository usuarioRepository)
        {
            _conversacionRepository = usuarioRepository;
        }

        public async Task<List<Conversacion>> ObtenerConversacionesPorUsuario(string username)
        {

            return await _conversacionRepository.ObtenerConversacionesPorUsuario(username);
        }

        public Task<string> ObtenerUltimoMensajeConversacion(Conversacion conversacion)
        {
            return Task.FromResult(conversacion.UltimoMensaje?.Texto ?? "Sin mensajes");
        }


    }
}

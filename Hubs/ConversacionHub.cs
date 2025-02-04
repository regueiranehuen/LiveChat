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

        public async Task<List<Mensaje>>ObtenerMensajesDeConversacion(string idConversacion)
        {
            return await _conversacionRepository.GetMensajesDeConversacion(idConversacion);
        }
     

    }
}

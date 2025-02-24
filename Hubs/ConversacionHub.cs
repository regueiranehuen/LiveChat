using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LiveChat
{
    public class ConversacionHub : Hub
    {
        private ConversacionRepository _conversacionRepository;


        public ConversacionHub(ConversacionRepository conversacionRepository)
        {
            _conversacionRepository = conversacionRepository;
        }

        

        public async Task<List<Conversacion>> ObtenerConversacionesPorUsuario(string username)
        {

            return await _conversacionRepository.ObtenerConversacionesPorUsuario(username);
        }

        

        


        public async Task <Conversacion?> ExisteConversacion(string usuario1, string usuario2)
        {
            return await _conversacionRepository.ExisteConversacion(usuario1, usuario2); // Si existe la conversacion simplemente la abrimos
        }

        public async Task<Conversacion?> CrearConversacion(string usuario1, string usuario2) // La conversacion puede crearse o no dependiendo de si el usuario buscado existe o no
        {
            UsuarioRepository usuarioRepository = new UsuarioRepository(new MongoDBConnection());

            if (await usuarioRepository.ObtenerUsuarioPorUsername(usuario2) == null) // solo chequeo el usuario2 porque el usuario1 ya esta logueado
            {
                return null; // El usuario buscado no existe y no se puede crear la conversacion
            }

            System.Diagnostics.Debug.WriteLine("JIJOLINES");
            return await _conversacionRepository.CrearConversacion(usuario1, usuario2); // El usuario buscado existe y se puede crear la conversacion

        }


    }
}

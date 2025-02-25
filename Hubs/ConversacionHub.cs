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

                                         // Clave, valor
        public static ConcurrentDictionary<string, string> Usuarios = new ConcurrentDictionary<string, string>();
        //public static ConcurrentDictionary<string, ConversacionHub> HubsConversaciones = new ConcurrentDictionary<string, ConversacionHub>(); // Con esto solo admito que el usuario tenga una pestaña abierta con las conversaciones funcionando bien
        // public static ConcurrentDictionary<string, List<ConversacionHub>> HubsConversaciones = new ConcurrentDictionary<string, List<ConversacionHub>>();  /// Si quisiera permitir varias pestañas

        public override async Task OnConnectedAsync()
        {
            string usuario = Context.User.Identity.Name; // Obtener usuario autenticado
            string connectionId = Context.ConnectionId; // Obtener ID de conexión

            System.Diagnostics.Debug.WriteLine("usuario sigma:" + usuario);
            System.Diagnostics.Debug.WriteLine("connection id sigma:" + connectionId);

            if (!string.IsNullOrEmpty(usuario))
            {
                Usuarios[usuario] = connectionId;
                //HubsConversaciones[usuario] = this;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Usuarios.TryRemove(Context.ConnectionId, out _);
            //HubsConversaciones.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
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

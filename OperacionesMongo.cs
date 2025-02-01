using MongoDB.Driver;

namespace LiveChat
{
    public class MongoDBConnection
    {
        private readonly IMongoDatabase database;

        public MongoDBConnection()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("ConnStringLiveChat"));
            database = client.GetDatabase("MENSAJERIA_DB");
        }

        public IMongoCollection<Usuario> GetUsuariosCollection()
        {
            return database.GetCollection<Usuario>("Usuarios");
        }

        public IMongoCollection<Conversacion> GetConversacionesCollection()
        {
            return database.GetCollection<Conversacion>("Conversaciones");
        }

    }

    public class UsuarioRepository
    {
        private readonly IMongoCollection<Usuario> usuariosCollection;

        public UsuarioRepository(MongoDBConnection connection)
        {
            usuariosCollection = connection.GetUsuariosCollection();
        }

        public async Task<Usuario> ObtenerUsuarioPorUsername(string username)
        {
            return await usuariosCollection.Find(u => u.Username == username).FirstOrDefaultAsync(); // Devuelve el primer usuario que coincida con el username o null
        }



        public async Task CrearUsuario(Usuario usuario)
        {
            await usuariosCollection.InsertOneAsync(usuario);
        }
    }

    public class ConversacionRepository
    {
        private readonly IMongoCollection<Conversacion> conversacionesCollection;
        public ConversacionRepository(MongoDBConnection connection)
        {
            conversacionesCollection = connection.GetConversacionesCollection();
        }



        public bool ExisteConversacion(string usuario1, string usuario2)
        {
            return conversacionesCollection.Find(c => c.Id == this.CrearIdConversacion(usuario1, usuario2) || 
                                                c.Id == this.CrearIdConversacion(usuario2,usuario1)).Any();
        }

        public string CrearIdConversacion(string usuario1, string usuario2)
        {
            return usuario1 + "," + usuario2;
        }

        public async Task CrearConversacion(string usuario1, string usuario2)
        {
            string idConversacion = this.CrearIdConversacion(usuario1, usuario2);
            Conversacion conversacion = new Conversacion(idConversacion, null); // Una conversacion sin mensajes
            await conversacionesCollection.InsertOneAsync(conversacion);
        }

        public async Task<List<Conversacion>> ObtenerConversacionesPorUsuario(string username)
        {
          
            return await conversacionesCollection.Find(c => c.Id.Contains(username + ",") ||
                                                        c.Id.Contains(","+username)).ToListAsync();
        }

        public async Task<Conversacion> ObtenerConversacionPorId(string id)
        {
            return await conversacionesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }
        /*public async Task AgregarMensajeAConversacion(string idConversacion, Mensaje mensaje)
        {
            var filter = Builders<Conversacion>.Filter.Eq(c => c.Id, idConversacion);
            var update = Builders<Conversacion>.Update.Push(c => c.Mensajes, mensaje);
            await conversacionesCollection.UpdateOneAsync(filter, update);
        }*/
    }
}

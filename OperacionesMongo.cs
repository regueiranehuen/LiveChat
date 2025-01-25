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
}

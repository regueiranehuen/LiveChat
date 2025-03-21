﻿using MongoDB.Driver;

namespace LiveChat
{

    
    public class MongoDBConnection : IDisposable // Implemento la interfaz IDisposable
    {
        private readonly IMongoDatabase database;
        private MongoClient? client;
        public MongoDBConnection()
        {
            client = new MongoClient(Environment.GetEnvironmentVariable("ConnStringLiveChat"));
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

        public IMongoCollection<Mensaje> GetMensajesCollection()
        {
            return database.GetCollection<Mensaje>("Mensajes");
        }

        // Con esto puedo hacer conexiones temporales manualmente usando using
        public void Dispose()
        {
            client = null;
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
        private readonly IMongoCollection<Mensaje> mensajesCollection;
        public ConversacionRepository(MongoDBConnection connection)
        {
            conversacionesCollection = connection.GetConversacionesCollection();
            mensajesCollection = connection.GetMensajesCollection();
        }



        public async Task<Conversacion?> ExisteConversacion(string usuario1, string usuario2)
        {
            return await conversacionesCollection.Find(c => c.Id == this.CrearIdConversacion(usuario1, usuario2) || 
                                                        c.Id == this.CrearIdConversacion(usuario2, usuario1)).FirstOrDefaultAsync();

        }
        
        public string CrearIdConversacion(string usuario1, string usuario2)
        {
            return usuario1 + "," + usuario2;
        }

        public async Task<Conversacion> CrearConversacion(string usuario1, string usuario2)
        {

            
            string idConversacion = this.CrearIdConversacion(usuario1, usuario2);
            Conversacion conversacion = new Conversacion(idConversacion, null); // Una conversacion sin mensajes
            await conversacionesCollection.InsertOneAsync(conversacion);

            return conversacion;
        }

        public async Task<List<Conversacion>> ObtenerConversacionesPorUsuario(string username)
        {
            List<Conversacion> conversaciones = await conversacionesCollection.Find(c => (c.Id.Contains(username + ",") ||
                                                        c.Id.Contains("," + username)) &&
                                                        c.UltimoMensaje != null). // Al usuario actual no le carga una conversacion que haya creado otra persona si es que nunca mandó un mensaje
                                                        SortByDescending(c => c.UltimoMensaje.Fecha).ToListAsync(); // Conversaciones ordenadas por fecha. 
            if (conversaciones.Count > 0)
                conversaciones.ForEach(conversacion => conversacion.UltimoMensaje.Fecha = conversacion.UltimoMensaje.Fecha.ToLocalTime());

            return conversaciones; 
        }

        public async Task<Conversacion?> ObtenerConversacionPorId(string id)
        {
            Conversacion? conversacion = await conversacionesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (conversacion.UltimoMensaje!=null)
                conversacion.UltimoMensaje.Fecha = conversacion.UltimoMensaje.Fecha.ToLocalTime();

            return conversacion;
        }
        public async Task AgregarMensajeAConversacion(string idConversacion, Mensaje mensaje)
        {
            var filter = Builders<Conversacion>.Filter.Eq(c => c.Id, idConversacion);
            var update = Builders<Conversacion>.Update.Set(c => c.UltimoMensaje, mensaje);
            await conversacionesCollection.UpdateOneAsync(filter, update); // Actualizamos la conversacion con el nuevo mensaje
        }

        public async Task AgregarMensajeAColeccionMensajes(Mensaje mensaje)
        {
            await mensajesCollection.InsertOneAsync(mensaje);
        }

        public async Task<List<Mensaje>> GetMensajesDeConversacion(string idConversacion)
        {
            List<Mensaje> mensajes = await this.mensajesCollection.Find(m => m.IdConversacion == idConversacion).ToListAsync();

            mensajes.ForEach(mensaje => mensaje.Fecha = mensaje.Fecha.ToLocalTime());

            return mensajes;
        }


    }

}

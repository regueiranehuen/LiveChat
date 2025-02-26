using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace LiveChat
{
    public class Conversacion
    {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonElement("ultimoMensaje")]
        public Mensaje? UltimoMensaje { get; set; } // Tipo nullable

        public Conversacion(string id, Mensaje? ultimoMensaje = null) // El ultimo mensaje puede ser null
        {
            Id = id;
            UltimoMensaje = ultimoMensaje;
        }
    }
}

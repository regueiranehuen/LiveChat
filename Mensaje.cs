using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiveChat
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Mongo me genera automáticamente el id
        public ObjectId Id { get; set; }

        [BsonElement("idConversacion")]
        public string IdConversacion { get; set; }

        [BsonElement("emisor")]
        public string Emisor { get; set; }

        [BsonElement("receptor")]
        public string Receptor { get; set; }

        [BsonElement("texto")]
        public string Texto { get; set; }

        [BsonElement("fecha")]
        [BsonRepresentation(BsonType.DateTime)] // Para que se guarde como DateTime en MongoDB
        public DateTime Fecha { get; set; }

        public Mensaje(string idConversacion, string emisor, string receptor, string texto, DateTime fecha)
        {
            IdConversacion = idConversacion;
            Emisor = emisor;
            Receptor = receptor;
            Texto = texto;
            Fecha = fecha;
        }

    }
}

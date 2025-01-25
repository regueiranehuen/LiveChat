using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LiveChat
{
    public class Usuario
    {
        [BsonId] // Indica que este será el campo "_id" en MongoDB
        [BsonRepresentation(BsonType.String)] // Permite usar strings en lugar de ObjectId
        public string Username { get; set; } // Este será el identificador único

        [BsonElement("passwordHash")] // Mapea a "passwordHash" en MongoDB
        public string PasswordHash { get; set; }

        [BsonElement("salt")] // Mapea a "salt" en MongoDB
        public string Salt { get; set; }

        public Usuario(string username, string passwordHash, string salt)
        {
            Username = username;
            PasswordHash = passwordHash;
            Salt = salt;
        }

    }
}

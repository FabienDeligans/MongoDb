using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbCore.Models
{
    public class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}

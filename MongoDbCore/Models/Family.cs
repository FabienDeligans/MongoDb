using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbCore.Models
{
    public class Family : Entity
    {
        public string FamilyName { get; set; }

        [BsonIgnore]
        public IEnumerable<Parent> Parents { get; set; }

        [BsonIgnore]
        public IEnumerable<Child> Children { get; set; }
    }
}

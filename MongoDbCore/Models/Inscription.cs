using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbCore.Models
{
    public class Inscription : Entity
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DayChoose { get; set; }

        public string ChildId { get; set; }

        public bool M { get; set; }
        public bool R { get; set; }
        public bool Am { get; set; }
    }
}

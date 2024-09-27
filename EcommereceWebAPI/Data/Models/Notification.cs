using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EcommereceWebAPI.Data.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }  

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }  

        [BsonElement("Message")]
        public string Message { get; set; }
    }
}

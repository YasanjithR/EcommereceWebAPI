using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class VendorRating
    {


        [BsonElement("CustomerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerID { get; set; }

        [BsonElement("Rating")]
        public int Rating { get; set; }

        [BsonElement("Comment")]
        public string? Comment { get; set; }

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}

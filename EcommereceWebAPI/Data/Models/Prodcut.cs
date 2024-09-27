using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class Prodcut
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Description ")]
        public string Description { get; set; }

        [BsonElement("Price")]
        public double Price { get; set; }

        [BsonElement("VendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorID {  get; set; }

        [BsonElement("Quantity")]
        public string Quantity { get; set; }

        [BsonElement("LowStockAlert")]
        public int LowStockAlert { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = true;



    }
}

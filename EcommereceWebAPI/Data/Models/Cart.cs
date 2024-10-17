using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CartId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("CustomerId")]
        public string CustomerId { get; set; }

        [BsonElement("CartItems")]
        public List<CartItems> CartItems { get; set; }

        [BsonElement("CartTotal")]
        public double CartTotal { get; set; }



    }


    public class CartItems
    {
        

        [BsonElement("ProductId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }


        [BsonElement("ProductName")]
        public string? ProductName { get; set; }

        [BsonElement("ProductImage")]
        public string? ProductImage { get; set; }

        [BsonElement("VendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }



        [BsonElement("Quantity")]
        public int Quantity { get; set; }  

        [BsonElement("Price")]
        public double Price { get; set; }
    }
}

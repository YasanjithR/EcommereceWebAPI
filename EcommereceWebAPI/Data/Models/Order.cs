using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
         public string? OrderId { get; set; }

        [BsonElement("CustomerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonElement("OrderStatus")]
        public string OrderStatus { get; set; }

        [BsonElement("OrderItems")]
        public List<OrderItem> OrderItems { get; set; }

        [BsonElement("OrderTotal")]
        public double OrderTotal {  get; set; }

        [BsonElement("isCancellationRequest")]
        public bool isCancellationRequest { get; set; }

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;


    }



    public class OrderItem
    {
        [BsonElement("ProductId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("VendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("Price")]
        public double Price { get; set; }

        [BsonElement("DelivaryStatus")]
        public string DelivaryStatus { get; set; } = "Pending";


    }
}

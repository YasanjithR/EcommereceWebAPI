using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }
        [BsonElement("Role")]
        public string? Role { get; set; }
        [BsonElement("IsActive")]
        public bool IsActive { get; set; } =true;

        [BsonElement("isApproved")]
        public bool isApproved { get; set; } 

        [BsonElement("VendorReviews")]
        public List<VendorRating>? VendorReviews { get; set; }

    }

}

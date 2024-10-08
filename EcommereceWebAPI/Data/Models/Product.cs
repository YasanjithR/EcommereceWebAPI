﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcommereceWebAPI.Data.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Category")]
        public string? Category { get; set; }

        [BsonElement("Description ")]
        public string? Description { get; set; }

        [BsonElement("ImageID ")]
        public string? ImageID { get; set; }

        [BsonElement("Price")]
        public double Price { get; set; }

        [BsonElement("VendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? VendorID {  get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("LowStockAlert")]
        public int LowStockAlert { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = true;



    }
}

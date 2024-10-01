using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcommereceWebAPI.Services
{
    public class OrderServce
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Order> _orders;

        public OrderServce(MongoDbContext context)
        {
            _context = context;
            _orders = _context.GetCollection<Order>("Order");
            
        }


        //public async Task<IActionResult> CreateOrder() { }
    }
}

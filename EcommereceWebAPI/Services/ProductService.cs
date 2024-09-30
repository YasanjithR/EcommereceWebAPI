using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcommereceWebAPI.Services
{
    public class ProductService
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<User> _user;

        public ProductService(MongoDbContext context)
        {
            _context = context;
            _products = _context.GetCollection<Product>("Product");
            _user = _context.GetCollection<User>("User");

        }

        public async Task<IActionResult> createProduct(Product product)
        {
            var vendorExists = await _user.Find(u=>u.Id==product.VendorID).FirstOrDefaultAsync();

            if(vendorExists == null)
            {
                return new NotFoundObjectResult(new { message = "Vendor Not Found" });
            }

            await _products.InsertOneAsync(product);
            return new OkObjectResult(new { message = "Product created successfully" });
        }
    }
}

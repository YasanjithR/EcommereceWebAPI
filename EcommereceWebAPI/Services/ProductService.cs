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


        public async Task<IActionResult> updateProduct(Product product)
        {

            try
            {
                var exists = await _products.Find(p => p.ProductId == product.ProductId).FirstOrDefaultAsync();

                if (exists == null)
                {

                    return new NotFoundObjectResult(new { message = "Product Not Found" });

                }

                await _products.ReplaceOneAsync(p => p.ProductId == product.ProductId, product);

                return new OkObjectResult(new { message = "Product updated successfully" });
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<IList<Product>> GetProductByVendor(string id)
        {

            try
            {
                var products = await _products.Find(p => p.VendorID == id).ToListAsync<Product>();

                if (products == null)
                {
                    return new List<Product>();
                }

                return products;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
               
                var product = await _products.Find(p => p.ProductId == id).FirstOrDefaultAsync();

              
                if (product == null)
                {
                    return new NotFoundObjectResult(new { message = "Product not found." });
                }

              
                return new OkObjectResult(product);
            }
            catch (Exception ex)
            {
               
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> DeleteProduct(string id)
        {

            try
            {
                var product = await _products.Find(p => p.ProductId == id).FirstOrDefaultAsync<Product>();


                if (product == null)
                {
                    return new NotFoundObjectResult(new { messeage = "Product Not Found" });
                }

                await _products.DeleteOneAsync(p => p.ProductId == id);

                return new OkObjectResult(new { message = "Product deleted successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

        } 

        public async Task<IList<Product>> GetAllProducts()
        {
            var products = await _products.Find(_ => true).ToListAsync();

            if (products == null)
            {
                return new List<Product>();
            }

            return products;
        }

    }
}

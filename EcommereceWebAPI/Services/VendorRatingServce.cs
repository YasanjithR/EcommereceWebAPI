using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;

namespace EcommereceWebAPI.Services
{
    public class VendorRatingServce
    {

        private readonly MongoDbContext _context;

        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Cart> _cart;
        private readonly IMongoCollection<Product> _product;

        public VendorRatingServce(MongoDbContext context)
        {
            _context = context;
            _users = _context.GetCollection<User>("User");
            _cart = _context.GetCollection<Cart>("Cart");
            _product = _context.GetCollection<Product>("Product");

        }

        public async Task<IActionResult> CreateVendorRating(string comment ,int rating,string vendorID)
        {
            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";
                VendorRating vendorRating = new VendorRating
                {

                    Comment = comment,
                    Rating = rating,
                    CustomerID = userId,
                    CreatedDate = DateTime.UtcNow,
                    

                };

                var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

             

                if (vendor == null)
                {
                    return new NotFoundObjectResult(new { message = "Vendor not found." + vendorID });
                }

                if (vendor.VendorReviews == null)
                {
                    vendor.VendorReviews = new List<VendorRating>();
                }

                vendor.VendorReviews.Add(vendorRating);

                var update = Builders<User>.Update.Set(u => u.VendorReviews, vendor.VendorReviews);

                await _users.UpdateOneAsync(c=>c.Id==vendorID, update);

                return new OkObjectResult(new { message = "Rating Created successfully" });


            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

        }

        public async Task<IActionResult> UpdateVendorRating(string comment, int rating, string vendorID)
        {
            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";  // Assume this is the logged-in user's ID

                
                var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

                if (vendor == null)
                {
                    return new NotFoundObjectResult(new { message = "Vendor not found. ID: " + vendorID });
                }

             
                if (vendor.VendorReviews == null || !vendor.VendorReviews.Any())
                {
                    return new NotFoundObjectResult(new { message = "No reviews found for this vendor." });
                }

                var existingRating = vendor.VendorReviews.FirstOrDefault(r => r.CustomerID == userId);

                if (existingRating == null)
                {
                    return new NotFoundObjectResult(new { message = "No rating found for this user on the vendor." });
                }

                
                existingRating.Rating = rating;
                existingRating.Comment = comment;
                existingRating.CreatedDate = DateTime.UtcNow; 

               
                var update = Builders<User>.Update.Set(u => u.VendorReviews, vendor.VendorReviews);
           

                await _users.UpdateOneAsync(u=>u.Id==vendorID, update);

                return new OkObjectResult(new { message = "Rating updated successfully." });
            }
            catch (Exception )
            {
                
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

        }


        public async Task<IActionResult> DeleteVendorReview(string vendorID)
        {
            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";  // Assume this is the logged-in user's ID

                var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

                if (vendor == null)
                {
                    return new NotFoundObjectResult(new { message = "Vendor not found. ID: " + vendorID });
                }


                if (vendor.VendorReviews == null || !vendor.VendorReviews.Any())
                {
                    return new NotFoundObjectResult(new { message = "No reviews found for this vendor." });
                }

                var existingRating = vendor.VendorReviews.FirstOrDefault(r => r.CustomerID == userId);

                if (existingRating == null)
                {
                    return new NotFoundObjectResult(new { message = "No rating found for this user on the vendor." });
                }
                vendor.VendorReviews.Remove(existingRating);

                var update = Builders<User>.Update.Set(u => u.VendorReviews, vendor.VendorReviews);

                await _users.UpdateOneAsync(u => u.Id == vendorID, update);

                return new OkObjectResult(new { message = "Rating deleted successfully." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }



        }



    }




}

using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using EcommereceWebAPI.Data.DTO;
using System.Runtime.InteropServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Numerics;
//this service is responsible for handling vendor rating related operations
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

        /// <summary>
        /// Creates a new vendor rating.
        /// </summary>
        /// <param name="userID">The ID of the user creating the rating.</param>
        /// <param name="comment">The comment for the rating.</param>
        /// <param name="rating">The rating value.</param>
        /// <param name="vendorID">The ID of the vendor being rated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CreateVendorRating(string userID, string comment, int rating, string vendorID)
        {
            try
            {
                var userId = userID;
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

                await _users.UpdateOneAsync(c => c.Id == vendorID, update);

                return new OkObjectResult(new { message = "Rating Created successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing vendor rating.
        /// </summary>
        /// <param name="userID">The ID of the user updating the rating.</param>
        /// <param name="comment">The updated comment for the rating.</param>
        /// <param name="rating">The updated rating value.</param>
        /// <param name="vendorID">The ID of the vendor being rated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> UpdateVendorRating(string userID, string comment, int rating, string vendorID)
        {
            try
            {
                var userId = userID;  // Assume this is the logged-in user's ID

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

                await _users.UpdateOneAsync(u => u.Id == vendorID, update);

                return new OkObjectResult(new { message = "Rating updated successfully." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Deletes a vendor review.
        /// </summary>
        /// <param name="userID">The ID of the user deleting the review.</param>
        /// <param name="vendorID">The ID of the vendor being reviewed.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> DeleteVendorReview(string userID, string vendorID)
        {
            try
            {
                var userId = userID;  // Assume this is the logged-in user's ID

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

        /// <summary>
        /// Retrieves all vendor ratings for a specific vendor.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>A list of VendorRating objects representing the vendor ratings.</returns>
        public async Task<IList<VendorRating>> ViewVendorRatings(string vendorID)
        {
            var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

            if (vendor == null)
            {
                return new List<VendorRating>();
            }

            var vendorRating = vendor.VendorReviews;

            if (vendorRating == null)
            {
                return new List<VendorRating>();
            }

            return vendorRating;
        }

        /// <summary>
        /// Retrieves all vendor ratings for a specific vendor by a specific customer.
        /// </summary>
        /// <param name="userID">The ID of the customer.</param>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>A list of VendorRating objects representing the vendor ratings.</returns>
        public async Task<IList<VendorRating>> ViewVendorRatingsByCustomer(string userID, string vendorID)
        {
            var userId = userID;  // Assume this is the logged-in user's ID

            var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

            if (vendor == null)
            {
                return new List<VendorRating>();
            }

            if (vendor.VendorReviews == null)
            {
                return new List<VendorRating>();
            }

            var vendorRating = vendor.VendorReviews.Where(c => c.CustomerID == userId).ToList();

            if (vendorRating == null)
            {
                return new List<VendorRating>();
            }

            return vendorRating;
        }

        /// <summary>
        /// Retrieves all customers who have provided vendor ratings.
        /// </summary>
        /// <param name="userID">The ID of the customer.</param>
        /// <returns>A list of User objects representing the customers.</returns>
        public async Task<IList<User>> CustomersVendorRatings(string userID)
        {
            try
            {
                var userId = userID;  // Assume this is the logged-in user's ID

                var vendorRating = await _users.Find(c => c.VendorReviews != null && c.VendorReviews.Any(r => r.CustomerID == userID)).ToListAsync();

                if (vendorRating == null || !vendorRating.Any())
                {
                    return new List<User>();
                }

                return vendorRating;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }




}

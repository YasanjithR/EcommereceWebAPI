using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcommereceWebAPI.Services
{
    public class UserService
    {

        private readonly MongoDbContext _context;
        
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Cart> _cart;

        public UserService(MongoDbContext context)
        {
            _context = context;
            _users = _context.GetCollection<User>("User");
            _cart = _context.GetCollection<Cart>("Cart");
        }

        public async Task<IActionResult> CreateUserAsync(User user)
        {
            try
            {
                var exists = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    return new ConflictObjectResult(new { message = "User already exists" });
                }

                await _users.InsertOneAsync(user);

                Cart userCart = new Cart
                {
                    CustomerId = user.Id
                };

                await _cart.InsertOneAsync(userCart);

                return new OkObjectResult(new { message = "User created successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}

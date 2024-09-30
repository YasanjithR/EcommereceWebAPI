using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using MongoDB.Driver;

namespace EcommereceWebAPI.Services
{
    public class UserService
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbContext context)
        {
            _context = context;
            _users = _context.GetCollection<User>("User");
        }

        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);  // Insert the user into the MongoDB "User" collection
        }
    }
}

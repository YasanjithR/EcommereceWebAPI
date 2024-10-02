using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Middleware;
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
        private readonly IMongoCollection<Product> _product;
        private readonly AuthService _authService;

        public UserService(MongoDbContext context,AuthService authService)
        {
            _context = context;
            _users = _context.GetCollection<User>("User");
            _cart = _context.GetCollection<Cart>("Cart");
            _product = _context.GetCollection<Product>("Product");
            _authService = authService;
        }

        public async Task<IActionResult> CreateAdminUserAsync(User user)
        {
            try
            {
                var exists = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    return new ConflictObjectResult(new { message = "User already exists" });
                }

                user.isApproved = true;
                user.Role = "Admin";

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

        public async Task<IActionResult> CreateCSRUserAsync(User user)
        {
            try
            {
                var exists = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    return new ConflictObjectResult(new { message = "User already exists" });
                }

                user.isApproved = true;
                user.Role = "CSR";

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


        public async Task<IActionResult> CreateVendorUserAsync(User user)
        {
            try
            {
                var exists = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    return new ConflictObjectResult(new { message = "User already exists" });
                }

                user.isApproved = true;
                user.Role = "Vendor";

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

        public async Task<IActionResult> CreateCustomerUserAsync(User user)
        {
            try
            {
                var exists = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();

                if (exists != null)
                {
                    return new ConflictObjectResult(new { message = "User already exists" });
                }

                user.isApproved = false;
                user.Role = "Customer";

                await _users.InsertOneAsync(user);

                Cart userCart = new Cart
                {
                    CustomerId = user.Id
                };

                await _cart.InsertOneAsync(userCart);

                //user activation notification for csr

                return new OkObjectResult(new { message = "User created successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> UserLogIn(string email, string password)
        {
            try
            {
                var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new NotFoundObjectResult(new { message = "User not found" });
                }


                if (user.PasswordHash != password)
                {
                    return new ConflictObjectResult(new { message = "Invalid Password" });
                }

                if (user.isApproved == false)
                {
                    return new ConflictObjectResult(new { message = "User Not active" });
                }


                var token = _authService.GenerateJwtToken(user.Id, user.Email, user.Role);

                return new OkObjectResult(new { token });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

        }


        public async Task<IActionResult> ActivateCustomer(string userID)
        {

            try
            {
                var user = await _users.Find(u => u.Id == userID).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new NotFoundObjectResult(new { message = "User not found" });
                }

                user.isApproved = true;

                var update = Builders<User>.Update.Set(u => u.isApproved, user.isApproved);

                await _users.UpdateOneAsync(u => u.Id == user.Id, update);

                //user activated nofication for customer

                return new OkObjectResult(new { message = "User approved successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }





        }


        public async Task<IActionResult> ActivateProduct(string id) {
            try
            {

                var product = await _product.Find(p => p.ProductId == id).FirstOrDefaultAsync();

                if (product == null)
                {

                    return new NotFoundObjectResult(new { message = "Product not found" });

                }

                product.IsActive = true;

                await _product.ReplaceOneAsync(p => p.ProductId == p.ProductId, product);

                return new OkObjectResult(new { message = "Product activated successfully" });

            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        public async Task<IList<User>> GetAllUsers()
        {
            try
            {
                var users = await _users.Find(_ => true).ToListAsync();

                if (users == null)
                {
                    return new List<User>();
                }

                return users;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<User> GetUserByID(string userID)
        {
            try
            {
                var user = await _users.Find(u => u.Id == userID).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new User();
                }

                return user;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommereceWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("InsertUser")]
        public async Task<User> CreateUser(User user)
        {
            await _userService.CreateUserAsync(user);
            return user; // Return 201 Created with the new user's details
        }
    }
}

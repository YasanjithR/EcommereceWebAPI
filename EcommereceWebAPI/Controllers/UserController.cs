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
        public async Task<IActionResult> CreateUser(User user)
        {
            try
            {
                var result = await _userService.CreateUserAsync(user);
                return result;

    }
            catch (Exception)
            {

                throw;
            }; // Return 201 Created with the new user's details
        }


        [HttpPatch]
        [Route("ActivateProduct/{id}")]
        public async Task<IActionResult> ActivateProduct(string id)
        {

            try
            {
                var result = await _userService.ActivateProduct(id);

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}

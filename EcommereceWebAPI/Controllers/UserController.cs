using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Route("CreateAdminUser")]
        public async Task<IActionResult> CreateAdminUser(User user)
        {
            try
            {
                var result = await _userService.CreateAdminUserAsync(user);
                return result;

    }
            catch (Exception)
            {

                throw;
            }; 
        }


        [HttpPost]
        [Route("CreateCSRUser")]
        public async Task<IActionResult> CreateCSRUser(User user)
        {
            try
            {
                var result = await _userService.CreateCSRUserAsync(user);
                return result;

            }
            catch (Exception)
            {

                throw;
            }; 
        }

        [HttpPost]
        [Route("CreateVendorUser")]
        public async Task<IActionResult> CreateVendorUser(User user)
        {
            try
            {
                var result = await _userService.CreateVendorUserAsync(user);
                return result;

            }
            catch (Exception)
            {

                throw;
            }; 
        }

        [HttpPost]
        [Route("CreateCustomerUser")]
        public async Task<IActionResult> CreateCustomerUser(User user)
        {
            try
            {
                var result = await _userService.CreateCustomerUserAsync(user);
                return result;

            }
            catch (Exception)
            {

                throw;
            }; 
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


        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _userService.UserLogIn(user.Email, user.PasswordHash);
            return result;
        }


        [Authorize]
        [HttpPatch]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody]User user)
        {
            var result = await _userService.UpdateUser(user);
            return result;
        }

        [Authorize(Roles ="CSR")]
        [HttpPatch]
        [Route("ApproveCustomer/{userID}")]

        public async Task<IActionResult> ApproveCustomer(string userID)
        {
            var result = await _userService.ApproveCustomer(userID);
            return result;
        }

        [Authorize]
        [HttpPatch]
        [HttpGet]
        [Route("GetAllUsers")]

        public async Task<IList<User>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return result;
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserByID/{userID}")]

        public async Task<User> GetUserByID(string userID)
        {
            var result = await _userService.GetUserByID(userID);
            return result;
        }

        [Authorize(Roles ="Customer,CSR,Admin")]
        [HttpPatch]
        [Route("DeactivateCustomerAccount/{userID}")]
        public async Task<IActionResult> DeactivateCustomerAccount(string userID)
        {
            var result = await _userService.DeactivateCustomerAccount(userID);
            return result;
        }


        [Authorize(Roles = "CSR")]
        [HttpPatch]
        [Route("ActivateCustomerAccount/{userID}")]
        public async Task<IActionResult> ActivateCustomerAccount(string userID)
        {
            var result = await _userService.ActivateCustomerAccount(userID);
            return result;
        }

        

    }
}

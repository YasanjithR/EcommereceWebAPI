using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//this controller is responsible for handling user related operations
namespace EcommereceWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates an admin user.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
            }
        }

        /// <summary>
        /// Creates a CSR (Customer Service Representative) user.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
            }
        }

        /// <summary>
        /// Creates a vendor user.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
            }
        }

        /// <summary>
        /// Creates a customer user.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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
            }
        }

        /// <summary>
        /// Activates a product.
        /// </summary>
        /// <param name="id">The ID of the product to be activated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="user">The user to be logged in.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _userService.UserLogIn(user.Email, user.PasswordHash);
            return result;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user to be updated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize]
        [HttpPatch]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var result = await _userService.UpdateUser(user);
            return result;
        }

        /// <summary>
        /// Approves a customer.
        /// </summary>
        /// <param name="userID">The ID of the customer to be approved.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "CSR")]
        [HttpPatch]
        [Route("ApproveCustomer/{userID}")]
        public async Task<IActionResult> ApproveCustomer(string userID)
        {
            var result = await _userService.ApproveCustomer(userID);
            return result;
        }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [Authorize]
        [HttpPatch]
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IList<User>> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return result;
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userID">The ID of the user to be retrieved.</param>
        /// <returns>The user with the specified ID.</returns>
        [Authorize]
        [HttpGet]
        [Route("GetUserByID/{userID}")]
        public async Task<User> GetUserByID(string userID)
        {
            var result = await _userService.GetUserByID(userID);
            return result;
        }

        /// <summary>
        /// Deactivates a customer account.
        /// </summary>
        /// <param name="userID">The ID of the customer account to be deactivated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Customer,CSR,Admin")]
        [HttpPatch]
        [Route("DeactivateCustomerAccount/{userID}")]
        public async Task<IActionResult> DeactivateCustomerAccount(string userID)
        {
            var result = await _userService.DeactivateCustomerAccount(userID);
            return result;
        }

        /// <summary>
        /// Activates a customer account.
        /// </summary>
        /// <param name="userID">The ID of the customer account to be activated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
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

using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
//this controller is responsible for handling cart related operations
namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("AddItemToCart")]
        // Adds an item to the cart.
        /// <summary>
        /// Adds an item to the cart.
        /// </summary>
        /// <param name="request">The cart item request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemRequestDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.addItemsToCart(userId, request.Product, request.Quantity);
            return result;
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete]
        [Route("DeleteFromCart")]
        // Deletes an item from the cart.
        /// <summary>
        /// Deletes an item from the cart.
        /// </summary>
        /// <param name="product">The product to be deleted.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> DeleteFromCart([FromBody] Product product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.DeleteCartItem(userId, product);
            return result;
        }

        [Authorize(Roles = "Customer")]
        [HttpPatch]
        [Route("UpdateCartItem")]
        // Updates a cart item.
        /// <summary>
        /// Updates a cart item.
        /// </summary>
        /// <param name="cartItemRequestDTO">The cart item request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemRequestDTO cartItemRequestDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.UpdateCartItem(userId, cartItemRequestDTO.Product.ProductId, cartItemRequestDTO.Quantity);
            return result;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("CheckOutCart")]
        // Checks out the cart and creates an order.
        /// <summary>
        /// Checks out the cart and creates an order.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CheckOutCart(string userID)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.CreateOrder(userId);

            if (result is OkObjectResult)
            {
                await ClearUserCart();
            }

            return result;
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete]
        [Route("ClearUserCart")]
        // Clears the user's cart.
        /// <summary>
        /// Clears the user's cart.
        /// </summary>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> ClearUserCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.ClearUserCart(userId);
            return result;
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        [Route("GetUserCart")]
        // Retrieves the user's cart.
        /// <summary>
        /// Retrieves the user's cart.
        /// </summary>
        /// <returns>The user's cart.</returns>
        public async Task<Cart> GetUserCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new Cart();
            }

            var result = await _cartService.GetUserCart(userId);
            return result;
        }
    }
}

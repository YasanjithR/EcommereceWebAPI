using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController :ControllerBase
    {

        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("AddItemToCart")]

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

        public async Task<IActionResult> DeleteFromCart([FromBody]Product product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            var result = await _cartService.DeleteCartItem(userId,product);
            return result;
        }
        [Authorize(Roles = "Customer")]
        [HttpPatch]
        [Route("UpdateCartItem")]

        public async Task<IActionResult> UpdateCartItem([FromBody]CartItemRequestDTO cartItemRequestDTO)
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

        public async Task<IActionResult> CheckOutCart(string userID)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _cartService.CreateOrder(userId);
            
            if(result is OkObjectResult)
            {
                await ClearUserCart();
            }

            return result;


        }
        [Authorize(Roles = "Customer")]
        [HttpDelete]
        [Route("ClearUserCart")]
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

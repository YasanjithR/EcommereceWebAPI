﻿using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("AddItemToCart")]

        public async Task<IActionResult> AddItemToCart([FromBody] CartItemRequestDTO request)
        {
            var result = await _cartService.addItemsToCart(request.Product, request.Quantity);
            return result;
        }

        [HttpDelete]
        [Route("DeleteFromCart")]

        public async Task<IActionResult> DeleteFromCart([FromBody]Product product)
        {
            var result = await _cartService.DeleteCartItem(product);
            return result;
        }

        [HttpPatch]
        [Route("UpdateCartItem")]

        public async Task<IActionResult> UpdateCartItem([FromBody]CartItemRequestDTO cartItemRequestDTO)
        {

            var result = await _cartService.UpdateCartItem(cartItemRequestDTO.Product.ProductId, cartItemRequestDTO.Quantity);
            return result;

        }


        [HttpPost]
        [Route("CheckOutCart")]

        public async Task<IActionResult> CheckOutCart()
        {

            var result = await _cartService.CreateOrder();
            
            if(result is OkObjectResult)
            {
                await ClearUserCart();
            }

            return result;


        }

        [HttpDelete]
        [Route("ClearUserCart")]
        public async Task<IActionResult> ClearUserCart()
        {

            var result = await _cartService.ClearUserCart();
            return result;
        }

        [HttpGet]
        [Route("GetUserCart")]

        public async Task<Cart> GetUserCart()
        {

            var result = await _cartService.GetUserCart();
            return result;

        }




    }
}

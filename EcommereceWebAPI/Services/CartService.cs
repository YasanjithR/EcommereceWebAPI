using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

namespace EcommereceWebAPI.Services
{
    public class CartService
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Cart> _cart;
        private readonly IMongoCollection<Product> _product;
        private readonly IMongoCollection<CartItems> _cartItems;
        private readonly IMongoCollection<Order> _order; 

        public CartService(MongoDbContext context)
        {
            _context = context;
            _cart = _context.GetCollection<Cart>("Cart");
            _cartItems = _context.GetCollection<CartItems>("CartItems");
            _product = _context.GetCollection<Product>("Product");
            _order = _context.GetCollection<Order>("Order");
        }

        public async Task<IActionResult> addItemsToCart(Product product, int quantity)
        {
            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();


                if (cart.CartItems == null)
                {
                    cart.CartItems = new List<CartItems>(); 
                }

                var exisitngCartItem = cart.CartItems.FirstOrDefault(p => p.ProductId == product.ProductId);

                if (exisitngCartItem != null)
                {
                    exisitngCartItem.Quantity = quantity;

                }
                else
                {

                    CartItems newCartitems = new CartItems();

                    newCartitems.Quantity = quantity;
                    newCartitems.ProductId = product.ProductId;
                    newCartitems.VendorId = product.VendorID;
                    newCartitems.Price = product.Price;

                    cart.CartItems.Add(newCartitems);

                }

                var update = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems);

                await _cart.UpdateOneAsync(c => c.CustomerId == userId, update);

                return new OkObjectResult(new { message = "Item added to cart successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        public async Task<IActionResult> DeleteCartItem(Product product)
        {
            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

                if (cart == null)
                {
                    return new NotFoundObjectResult(new { message = "Cart not found." });
                }

           
                if (cart.CartItems == null)
                {
                    cart.CartItems = new List<CartItems>();
                }

                var item = cart.CartItems.FirstOrDefault(i => i.ProductId == product.ProductId);

                if (item == null)
                {
                    return new NotFoundObjectResult(new { message = "Item not in cart." });
                }

                cart.CartItems.Remove(item);

                var update = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems);
                await _cart.UpdateOneAsync(c => c.CustomerId == userId, update);

                return new OkObjectResult(new { message = "Item removed from cart successfully." });
            }
            catch (Exception)
            {
                
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }


        public async Task<IActionResult> UpdateCartItem(string productID, int quantity)
        {

            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();


                if (cart.CartItems == null)
                {
                    return new NotFoundObjectResult(new { message = "Item not in cart." });
                }

                var item = cart.CartItems.FirstOrDefault(p => p.ProductId == productID);

                if (item == null)
                {

                    return new NotFoundObjectResult(new { message = "Item not in cart." });

                }

                item.Quantity = quantity;

                var update = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems);

                await _cart.UpdateOneAsync(c => c.CustomerId == userId, update);

                return new OkObjectResult(new { message = "Item updated successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

            
        
          
        }

        public async Task<IActionResult> CreateOrder()
        {
            try
            {
                Order order = new Order
                {
                    OrderItems = new List<OrderItem>()
                };


                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();


                if (cart == null)
                {
                    return new NotFoundObjectResult(new { message = "Cart not found." });
                }

                if (cart.CartItems == null)
                {
                    return new NotFoundObjectResult(new { message = "No Items in Cart" });
                }


                foreach (var item in cart.CartItems)
                {
                    OrderItem orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        VendorId = item.VendorId,
                        Quantity = item.Quantity,
                        Price = item.Price * item.Quantity,

                    };

                    order.OrderItems.Add(orderItem);
                    order.OrderTotal += orderItem.Price;

                }

                order.OrderStatus = "Placed";
                order.CustomerId = userId;
                order.CreatedDate = DateTime.UtcNow;

                await _order.InsertOneAsync(order);

                return new OkObjectResult(new { message = "Order Placed successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
               
                throw;
            }

        }

        public async Task<IActionResult> ClearUserCart()
        {

            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();


                if (cart == null)
                {
                    return new NotFoundObjectResult(new { message = "Cart not found." });
                }

                if (cart.CartItems == null)
                {
                    return new NotFoundObjectResult(new { message = "No Items in Cart" });
                }


                cart.CartItems.Clear();

                var update = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems);

                await _cart.UpdateOneAsync(c => c.CustomerId == userId, update);

                return new OkObjectResult(new { message = "Cart Cleared successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }

        }



        public async Task<Cart> GetUserCart()
        {


            try
            {
                var userId = "66fa83a39b6a7770ba6798e7";

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();


                if (cart == null)
                {
                    return new Cart();

                }

                return cart;

            }
            catch (Exception)
            {
                
                throw;
            }

        }
    }
   

}


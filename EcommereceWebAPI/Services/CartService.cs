using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

//this service class is used to handle the cart operations like adding items to cart, deleting items from cart, updating items in cart, creating order from cart, clearing cart and getting user cart
namespace EcommereceWebAPI.Services
{
    public class CartService
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Cart> _cart;
        private readonly IMongoCollection<Product> _product;
        private readonly IMongoCollection<CartItems> _cartItems;
        private readonly IMongoCollection<Order> _order;
        private readonly NotificationService _notificationService;

        public CartService(MongoDbContext context, NotificationService notificationService)
        {
            _context = context;
            _cart = _context.GetCollection<Cart>("Cart");
            _cartItems = _context.GetCollection<CartItems>("CartItems");
            _product = _context.GetCollection<Product>("Product");
            _order = _context.GetCollection<Order>("Order");
            _notificationService = notificationService;
        }

        /// <summary>
        /// Adds items to the cart.
        /// </summary>
        /// <param name="userID">The ID of the user adding the items.</param>
        /// <param name="product">The product to be added.</param>
        /// <param name="quantity">The quantity of the product to be added.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> addItemsToCart(string userID, Product product, int quantity)
        {
            try
            {
                var userId = userID;

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

                if (cart == null)
                {
                    return new NotFoundObjectResult(new { message = "no cart." });
                }


                if (cart.CartItems == null)
                {
                    cart.CartItems = new List<CartItems>();
                }

                var exisitngCartItem = cart.CartItems.FirstOrDefault(p => p.ProductId == product.ProductId);

                if (exisitngCartItem != null)
                {

                    if (product.Quantity < quantity)
                    {
                        return new ConflictObjectResult(new { message = "Not enough stock available" });
                    }

                    exisitngCartItem.Quantity = quantity;

                }
                else
                {
                    if (product.Quantity < quantity)
                    {
                        return new ConflictObjectResult(new { message = "Order stock not available" });
                    }

                    CartItems newCartitems = new CartItems();

                    newCartitems.Quantity = quantity;
                    newCartitems.ProductId = product.ProductId;
                    newCartitems.VendorId = product.VendorID;
                    newCartitems.Price = product.Price;

                    cart.CartItems.Add(newCartitems);
                    cart.CartTotal += newCartitems.Quantity * newCartitems.Price;

                    product.Quantity = product.Quantity - quantity;

                    if (product.Quantity < product.LowStockAlert)
                    {
                        //notifiction to venor
                        Notification notification = new Notification();
                        notification.UserId = product.VendorID;
                        notification.Type = "Stocks low alert";
                        notification.Message = "stocks are low for the product " + product.ProductId;

                        await _notificationService.CreateNotification(notification);
                    }

                }

                var cartUpdate = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems).Set(c => c.CartTotal, cart.CartTotal);

                var productUpdate = Builders<Product>.Update.Set(p => p.Quantity, product.Quantity);


                await _cart.UpdateOneAsync(c => c.CustomerId == userId, cartUpdate);
                await _product.UpdateOneAsync(p => p.ProductId == product.ProductId, productUpdate);


                return new OkObjectResult(new { message = "Item added to cart successfully" });
            }
            catch (Exception)
            {
                //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Deletes a cart item.
        /// </summary>
        /// <param name="userID">The ID of the user deleting the item.</param>
        /// <param name="product">The product to be deleted.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> DeleteCartItem(string userID, Product product)
        {
            try
            {
                var userId = userID;

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
                cart.CartTotal -= item.Quantity * item.Price;

                product.Quantity += item.Quantity;

                var cartUpdate = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems).Set(c => c.CartTotal, cart.CartTotal);

                var productUpdate = Builders<Product>.Update.Set(p => p.Quantity, product.Quantity);

                await _cart.UpdateOneAsync(c => c.CustomerId == userId, cartUpdate);
                await _product.UpdateOneAsync(p => p.ProductId == product.ProductId, productUpdate);

                return new OkObjectResult(new { message = "Item removed from cart successfully." });
            }
            catch (Exception)
            {

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Updates a cart item.
        /// </summary>
        /// <param name="userID">The ID of the user updating the item.</param>
        /// <param name="productID">The ID of the product to be updated.</param>
        /// <param name="quantity">The new quantity of the product.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> UpdateCartItem(string userID, string productID, int quantity)
        {

            try
            {
                var userId = userID;

                var cart = await _cart.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

                var product = await _product.Find(p => p.ProductId == productID).FirstOrDefaultAsync();

                if (product == null)
                {
                    return new NotFoundObjectResult(new { message = "product not found." });
                }

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

                if (product.Quantity < quantity)
                {
                    return new ConflictObjectResult(new { message = "Order stock not available" });
                }

                cart.CartTotal = cart.CartItems.Sum(i => i.Quantity * i.Price);


                var update = Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems).Set(c => c.CartTotal, cart.CartTotal);

                await _cart.UpdateOneAsync(c => c.CustomerId == userId, update);

                return new OkObjectResult(new { message = "Item updated successfully" });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }




        }

        /// <summary>
        /// Creates an order.
        /// </summary>
        /// <param name="userID">The ID of the user creating the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CreateOrder(string userID)
        {
            try
            {
                Order order = new Order
                {
                    OrderItems = new List<OrderItem>()
                };


                var userId = userID;

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

        /// <summary>
        /// Clears the user's cart.
        /// </summary>
        /// <param name="userID">The ID of the user whose cart will be cleared.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> ClearUserCart(string userID)
        {

            try
            {
                var userId = userID;

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

        /// <summary>
        /// Retrieves the user's cart.
        /// </summary>
        /// <param name="userID">The ID of the user whose cart will be retrieved.</param>
        /// <returns>The user's cart.</returns>
        public async Task<Cart> GetUserCart(string userID)
        {


            try
            {
                var userId = userID;

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


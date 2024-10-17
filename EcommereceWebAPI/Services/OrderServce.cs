using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Formats.Asn1;
//this service is responsible for handling order related operations
namespace EcommereceWebAPI.Services
{
    public class OrderServce
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<User> _users;
        private readonly NotificationService _notificationService;

        public OrderServce(MongoDbContext context, NotificationService notificationService)
        {
            _context = context;
            _orders = _context.GetCollection<Order>("Order");
            _users = _context.GetCollection<User>("User");
            _notificationService = notificationService;
        }

        // View customer orders
        /// <summary>
        /// Retrieves a list of orders for a specific customer.
        /// </summary>
        /// <param name="userID">The ID of the customer.</param>
        /// <returns>A list of orders.</returns>
        public async Task<IList<Order>> ViewCustomerOrders(string userID)
        {
            try
            {
                var user = await _users.Find(u => u.Id == userID).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new List<Order>();
                }

                var userOrders = await _orders.Find(o => o.CustomerId == userID).ToListAsync();

                if (userOrders == null)
                {
                    return new List<Order>();
                }

                return userOrders;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // View vendor orders
        /// <summary>
        /// Retrieves a list of orders for a specific vendor.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>A list of orders.</returns>
        public async Task<IList<Order>> ViewVendorOrders(string vendorID)
        {
            try
            {
                var user = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

                if (user == null)
                {
                    return new List<Order>();
                }

                var vendorOrders = await _orders.Find(o => o.OrderItems.Any(i => i.VendorId == vendorID)).ToListAsync();

                if (vendorOrders == null)
                {
                    return new List<Order>();
                }

                return vendorOrders;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Update order item status
        /// <summary>
        /// Updates the status of an order item.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <param name="newStatus">The new status of the order item.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> UpdateOrderItemStatus(string orderId, string productId, string vendorId, string newStatus)
        {
            var order = await _orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

            if (order == null)
            {
                return new NotFoundObjectResult(new { message = "Order not found." });
            }

            var orderItem = order.OrderItems.FirstOrDefault(i => i.ProductId == productId && i.VendorId == vendorId);

            if (orderItem == null)
            {
                return new NotFoundObjectResult(new { message = "Order item not found for the vendor." });
            }

            // Update the delivery status of the product for the vendor
            orderItem.DelivaryStatus = newStatus;

            // If all items are marked as "Delivered", update the overall order status
            if (order.OrderItems.All(i => i.DelivaryStatus == "Delivered"))
            {
                order.OrderStatus = "Delivered";

                // Notification to customer
                Notification notification = new Notification();
                notification.UserId = order.CustomerId;
                notification.Type = "Order Delivered";
                notification.Message = "Your order has been delivered " + order.OrderId;

                await _notificationService.CreateNotification(notification);
            }
            else if (order.OrderItems.Any(i => i.DelivaryStatus == "Delivered"))
            {
                order.OrderStatus = "Partially Delivered";
            }

            var update = Builders<Order>.Update
                .Set(o => o.OrderItems, order.OrderItems)
                .Set(o => o.OrderStatus, order.OrderStatus);

            await _orders.UpdateOneAsync(o => o.OrderId == orderId, update);

            return new OkObjectResult(new { message = "Order item status updated successfully." });
        }

        // Mark order as delivered
        /// <summary>
        /// Marks an order as delivered.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> MarkOrderAsDelivered(string orderId)
        {
            try
            {
                var order = await _orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

                if (order == null)
                {
                    return new NotFoundObjectResult(new { message = "Order not found." });
                }

                // Update the status of all items to "Delivered"
                foreach (var item in order.OrderItems)
                {
                    item.DelivaryStatus = "Delivered";
                }

                // Set the overall order status to "Delivered"
                order.OrderStatus = "Delivered";

                var update = Builders<Order>.Update
                    .Set(o => o.OrderItems, order.OrderItems)
                    .Set(o => o.OrderStatus, order.OrderStatus);

                await _orders.UpdateOneAsync(o => o.OrderId == orderId, update);

                // Notification to customer
                Notification notification = new Notification();
                notification.UserId = order.CustomerId;
                notification.Type = "Order Delivered";
                notification.Message = "Your order has been delivered " + order.OrderId;

                await _notificationService.CreateNotification(notification);

                return new OkObjectResult(new { message = "Order marked as delivered." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        // Order cancel request
        /// <summary>
        /// Sends a cancel request for an order.
        /// </summary>
        /// <param name="userID">The ID of the user.</param>
        /// <param name="orderID">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> OrderCancelRequest(string userID, string orderID)
        {
            try
            {
                var userId = userID;  // Assume this is the logged-in user's ID

                var order = await _orders.Find(o => o.OrderId == orderID && o.CustomerId == userId).FirstOrDefaultAsync();

                if (order == null)
                {
                    return new NotFoundObjectResult(new { message = "Order not found." });
                }

                if (order.OrderStatus == "Delivered" && order.OrderStatus == "Dispatched")
                {
                    return new ConflictObjectResult(new { message = "Order already dispatched." });
                }

                order.isCancellationRequest = true;

                var update = Builders<Order>.Update.Set(o => o.isCancellationRequest, order.isCancellationRequest);

                await _orders.UpdateOneAsync(o => o.OrderId == orderID, update);

                // Notification to CSR
                Notification notification = new Notification();

                var csr = await _users.Find(u => u.Role == "CSR").FirstOrDefaultAsync();

                notification.UserId = csr.Id;
                notification.Type = "Order Cancel Request";
                notification.Message = "Please cancel Order " + order.OrderId;

                await _notificationService.CreateNotification(notification);

                return new OkObjectResult(new { message = "Order cancel request made." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        // View cancel order requests
        /// <summary>
        /// Retrieves a list of cancel order requests.
        /// </summary>
        /// <returns>A list of orders.</returns>
        public async Task<IList<Order>> ViewCancelOrderRequest()
        {
            try
            {
                var cancelledOrders = await _orders.Find(o => o.isCancellationRequest == true).ToListAsync();

                if (cancelledOrders == null)
                {
                    return new List<Order>();
                }

                return cancelledOrders;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Cancel order
        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderID">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CancelOrder(string orderID)
        {
            try
            {
                var cancelOrder = await _orders.Find(o => o.OrderId == orderID).FirstOrDefaultAsync();

                if (cancelOrder == null)
                {
                    return new NotFoundObjectResult(new { message = "Order not found." });
                }

                if (cancelOrder.isCancellationRequest == false)
                {
                    return new ConflictObjectResult(new { message = "Order not set to be cancelled." });
                }

                // Notification to Customer
                Notification notification = new Notification();
                notification.UserId = cancelOrder.CustomerId;
                notification.Type = "Order Cancelled";
                notification.Message = "Your order has been cancelled " + cancelOrder.OrderId;

                await _notificationService.CreateNotification(notification);

                await _orders.DeleteOneAsync(o => o.OrderId == cancelOrder.OrderId);

                return new OkObjectResult(new { message = "Order is cancelled." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }

        /// <summary>
        /// Creates a new vendor rating.
        /// </summary>
        /// <param name="userID">The ID of the user creating the rating.</param>
        /// <param name="comment">The comment for the rating.</param>
        /// <param name="rating">The rating value.</param>
        /// <param name="vendorID">The ID of the vendor being rated.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CreateVendorRating(string userID, string comment, int rating, string vendorID)
        {
            try
            {
                // Create a new VendorRating object
                VendorRating vendorRating = new VendorRating
                {
                    CustomerID = userID,
                    Comment = comment,
                    Rating = rating,
                    CreatedDate = DateTime.Now
                };

                // Find the vendor
                var vendor = await _users.Find(u => u.Id == vendorID).FirstOrDefaultAsync();

                if (vendor == null)
                {
                    return new NotFoundObjectResult(new { message = "Vendor not found." });
                }

                // Add the rating to the vendor's reviews
                if (vendor.VendorReviews == null)
                {
                    vendor.VendorReviews = new List<VendorRating>();
                }

                vendor.VendorReviews.Add(vendorRating);

                // Update the vendor
                var update = Builders<User>.Update.Set(u => u.VendorReviews, vendor.VendorReviews);

                await _users.UpdateOneAsync(u => u.Id == vendorID, update);

                return new OkObjectResult(new { message = "Vendor rating created successfully." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                throw;
            }
        }
    }


}

using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Formats.Asn1;

namespace EcommereceWebAPI.Services
{
    public class OrderServce
    {

        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<User> _users;

        public OrderServce(MongoDbContext context)
        {
            _context = context;
            _orders = _context.GetCollection<Order>("Order");
            _users = _context.GetCollection<User>("User");


        }


     


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


        //public async Task<VendorOrderItemDTO> ViewOrderVendorItems (string vendorID)
        //{

        //}


        //public async Task<IActionResult> UpdateOrderStatus()

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
            orderItem.DelivaryStatus = newStatus;  // "Ready", "Delivered", etc.

            // If all items are marked as "Delivered", update the overall order status
            if (order.OrderItems.All(i => i.DelivaryStatus == "Delivered"))
            {
                order.OrderStatus = "Delivered";
                
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



        public async Task<IActionResult> MarkOrderAsDelivered(string orderId)
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

            return new OkObjectResult(new { message = "Order marked as delivered." });
        }


    }


}

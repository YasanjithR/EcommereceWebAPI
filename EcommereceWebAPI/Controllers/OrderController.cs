using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
//this controller is responsible for handling order related operations
namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderServce _orderService;

        public OrderController(OrderServce orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Retrieves a list of customer orders.
        /// </summary>
        /// <param name="userID">The ID of the customer.</param>
        /// <returns>A list of customer orders.</returns>
        [Authorize(Roles = "Vendor,Customer")]
        [HttpGet]
        [Route("ViewCustomerOrders/{userID}")]
        public async Task<IList<Order>> ViewCustomerOrders(string userID)
        {
            var result = await _orderService.ViewCustomerOrders(userID);
            return result;
        }

        /// <summary>
        /// Retrieves a list of vendor orders.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>A list of vendor orders.</returns>
        [Authorize(Roles = "Vendor")]
        [HttpGet]
        [Route("ViewVendorOrder/{vendorID}")]
        public async Task<IList<Order>> ViewVendorOrder(string vendorID)
        {
            var result = await _orderService.ViewVendorOrders(vendorID);
            return result;
        }

        /// <summary>
        /// Updates the status of a vendor order item.
        /// </summary>
        /// <param name="orderItemDTO">The DTO containing the order item details.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Vendor")]
        [HttpPatch]
        [Route("UpdateOrderVendorItemStatus")]
        public async Task<IActionResult> UpdateOrderItemStatus([FromBody] VendorOrderItemDTO orderItemDTO)
        {
            var result = await _orderService.UpdateOrderItemStatus(orderItemDTO.OrderId, orderItemDTO.ProductId, orderItemDTO.VendorId, orderItemDTO.Status);
            return result;
        }

        /// <summary>
        /// Marks an order as delivered.
        /// </summary>
        /// <param name="orderID">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("MarkOrderDelivered/{orderID}")]
        public async Task<IActionResult> MarkOrderDelivered(string orderID)
        {
            var result = await _orderService.MarkOrderAsDelivered(orderID);
            return result;
        }

        /// <summary>
        /// Requests cancellation of an order by a customer.
        /// </summary>
        /// <param name="orderID">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPatch]
        [Route("OrderCancelRequest/{orderID}")]
        public async Task<IActionResult> OrderCancelRequest(string orderID)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _orderService.OrderCancelRequest(userId, orderID);
            return result;
        }

        /// <summary>
        /// Retrieves a list of cancel order requests.
        /// </summary>
        /// <returns>A list of cancel order requests.</returns>
        [Authorize(Roles = "CSR")]
        [HttpGet]
        [Route("ViewCancelOrderRequest")]
        public async Task<IList<Order>> ViewCancelOrderRequest()
        {
            var result = await _orderService.ViewCancelOrderRequest();
            return result;
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderID">The ID of the order.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "CSR")]
        [HttpDelete]
        [Route("CancelOrder/{orderID}")]
        public async Task<IActionResult> CancelOrder(string orderID)
        {
            var result = await _orderService.CancelOrder(orderID);
            return result;
        }
    }

}

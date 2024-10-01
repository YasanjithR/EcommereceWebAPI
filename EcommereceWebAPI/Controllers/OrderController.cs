using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController:ControllerBase
    {

        private readonly OrderServce _orderService;

        public OrderController(OrderServce orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("ViewCustomerOrders/{userID}")]
        public async Task<IList<Order>> ViewCustomerOrders(string userID)
        {
            var result = await _orderService.ViewCustomerOrders(userID);
            return result;
        }


        [HttpGet]
        [Route("ViewVendorOrder/{vendorID}")]
        public async Task<IList<Order>> ViewVendorOrder(string vendorID)
        {
            var result = await _orderService.ViewVendorOrders(vendorID);
            return result;
        }


        [HttpPatch]
        [Route("MarkOrderDelivered/{orderID}")]
        public async Task<IActionResult> MarkOrderDelivered(string orderID)
        {
            var result = await _orderService.MarkOrderAsDelivered(orderID);
            return result;
        }
    }

}

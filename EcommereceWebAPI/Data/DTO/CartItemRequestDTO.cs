using EcommereceWebAPI.Data.Models;

namespace EcommereceWebAPI.Data.DTO
{
    public class CartItemRequestDTO
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}

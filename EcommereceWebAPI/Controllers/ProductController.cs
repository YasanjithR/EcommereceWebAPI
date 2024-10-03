using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController:ControllerBase
    {

        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }


        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct(Product product)
        {

           var result = await _productService.createProduct(product);
            return result;

        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPatch]
        [Route("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var result  = await _productService.updateProduct(product);

            return result;
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpGet]
        [Route("GetProductsByVendor/{id}")]

        public async Task<IList<Product>> GetProductsByVendor(string id)
        {
            var results = await _productService.GetProductByVendor(id);
            return results;
        }

        [HttpGet]
        [Route("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var result = await _productService.GetProductById(id);
            return result; 
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _productService.DeleteProduct(id);
            return result;
        }

    }
}

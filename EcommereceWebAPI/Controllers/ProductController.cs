using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//this controller is responsible for handling product related operations
namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <returns>The created product.</returns>
        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct(Product product)
        {

            var result = await _productService.createProduct(product);
            return result;

        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <returns>The updated product.</returns>
        [Authorize(Roles = "Admin,Vendor")]
        [HttpPatch]
        [Route("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var result = await _productService.updateProduct(product);

            return result;
        }

        /// <summary>
        /// Retrieves all products by vendor.
        /// </summary>
        /// <param name="id">The vendor ID.</param>
        /// <returns>A list of products.</returns>
        [Authorize(Roles = "Admin,Vendor")]
        [HttpGet]
        [Route("GetProductsByVendor/{id}")]
        public async Task<IList<Product>> GetProductsByVendor(string id)
        {
            var results = await _productService.GetProductByVendor(id);
            return results;
        }

        /// <summary>
        /// Retrieves a product by ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product.</returns>
        [HttpGet]
        [Route("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var result = await _productService.GetProductById(id);
            return result;
        }

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The result of the deletion.</returns>
        [Authorize(Roles = "Admin,Vendor")]
        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _productService.DeleteProduct(id);
            return result;
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [Authorize]
        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IList<Product>> GetAllProducts()
        {
            var result = await _productService.GetAllProducts();
            return result;
        }

    }
}

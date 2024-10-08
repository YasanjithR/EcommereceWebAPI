using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using EcommereceWebAPI.Data.Models;  

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorRatingController : ControllerBase
    {

        private readonly VendorRatingServce _vendorRatingServce;

        public VendorRatingController(VendorRatingServce vendorRatingServce)
        {
            _vendorRatingServce = vendorRatingServce;
        }

        /// <summary>
        /// Creates a new vendor rating.
        /// </summary>
        /// <param name="vendorRatingDTO">The vendor rating data.</param>
        /// <returns>The result of the operation.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [Route("CreateRating")]
        public async Task<IActionResult> CreateRating([FromBody] VendorRatingDTO vendorRatingDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _vendorRatingServce.CreateVendorRating(userId, vendorRatingDTO.Comment, vendorRatingDTO.Rating, vendorRatingDTO.VendorID);
            return result;
        }

        /// <summary>
        /// Updates an existing vendor rating.
        /// </summary>
        /// <param name="vendorRatingDTO">The updated vendor rating data.</param>
        /// <returns>The result of the operation.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPatch]
        [Route("UpdateRating")]
        public async Task<IActionResult> UpdateRating([FromBody] VendorRatingDTO vendorRatingDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _vendorRatingServce.UpdateVendorRating(userId, vendorRatingDTO.Comment, vendorRatingDTO.Rating, vendorRatingDTO.VendorID);
            return result;
        }

        /// <summary>
        /// Deletes a vendor review.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>The result of the operation.</returns>
        [Authorize(Roles = "Customer")]
        [HttpPatch]
        [Route("DeleteVendorReview/{vendorID}")]
        public async Task<IActionResult> DeleteVendorReview(string vendorID)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _vendorRatingServce.DeleteVendorReview(userId, vendorID);
            return result;
        }

        /// <summary>
        /// Retrieves the vendor ratings for a specific vendor.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>The list of vendor ratings.</returns>
        [Authorize]
        [HttpGet]
        [Route("ViewVendorRatings/{vendorID}")]
        public async Task<IList<VendorRating>> ViewVendorRatings(string vendorID)
        {
            var ratings = await _vendorRatingServce.ViewVendorRatings(vendorID);

            return ratings;
        }

        /// <summary>
        /// Retrieves the vendor ratings for a specific vendor by a specific customer.
        /// </summary>
        /// <param name="vendorID">The ID of the vendor.</param>
        /// <returns>The list of vendor ratings.</returns>
        [Authorize]
        [HttpGet]
        [Route("ViewVendorRatingsByCustomer/{vendorID}")]
        public async Task<IList<VendorRating>> ViewVendorRatingsByCustomer(string vendorID)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new List<VendorRating>();
            }

            var ratings = await _vendorRatingServce.ViewVendorRatingsByCustomer(userId, vendorID);

            return ratings;
        }

        /// <summary>
        /// Retrieves the vendor ratings given by the current customer.
        /// </summary>
        /// <returns>The list of vendor ratings.</returns>
        [Authorize]
        [HttpGet]
        [Route("CustomersVendorRatings")]
        public async Task<IList<User>> ViewCustomerVendorRating()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new List<User>();
            }

            var ratings = await _vendorRatingServce.CustomersVendorRatings(userId);

            return ratings;
        }
    }
}

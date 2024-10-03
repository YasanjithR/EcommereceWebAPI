using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using EcommereceWebAPI.Data.Models;  // For your User model

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorRatingController:ControllerBase
    {

        private readonly VendorRatingServce _vendorRatingServce;

        public VendorRatingController (VendorRatingServce vendorRatingServce)
        {
            _vendorRatingServce = vendorRatingServce;
        }

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
            var result = await _vendorRatingServce.CreateVendorRating(userId, vendorRatingDTO.Comment,vendorRatingDTO.Rating,vendorRatingDTO.VendorID);
            return result;
        }

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

        [Authorize]
        [HttpGet]
        [Route("ViewVendorRatings/{vendorID}")]
        public async Task<IList<VendorRating>> ViewVendorRatings(string vendorID)
        {
           
            var ratings = await _vendorRatingServce.ViewVendorRatings(vendorID);          

            return ratings;

        }

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

    }
}

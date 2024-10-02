using EcommereceWebAPI.Data.DTO;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorRatingController
    {

        private readonly VendorRatingServce _vendorRatingServce;

        public VendorRatingController (VendorRatingServce vendorRatingServce)
        {
            _vendorRatingServce = vendorRatingServce;
        }


        [HttpPost]
        [Route("CreateRating")]

        public async Task<IActionResult> CreateRating([FromBody] VendorRatingDTO vendorRatingDTO)
        {

            var result = await _vendorRatingServce.CreateVendorRating(vendorRatingDTO.Comment,vendorRatingDTO.Rating,vendorRatingDTO.VendorID);
            return result;
        }

        [HttpPatch]
        [Route("UpdateRating")]
        public async Task<IActionResult> UpdateRating([FromBody] VendorRatingDTO vendorRatingDTO)
        {
            var result = await _vendorRatingServce.UpdateVendorRating(vendorRatingDTO.Comment, vendorRatingDTO.Rating, vendorRatingDTO.VendorID);
            return result;
        }

        [HttpPatch]
        [Route("DeleteVendorReview/{vendorID}")]
        public async Task<IActionResult> DeleteVendorReview(string vendorID)
        {
            var result = await _vendorRatingServce.DeleteVendorReview(vendorID);
            return result;
        }


    }
}

using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class S3Controller : Controller
    {
        private readonly S3Service _s3Service;

        public S3Controller(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpGet("upload-url")]
        public async Task<IActionResult> GetUploadURL()
        {
            var url = await _s3Service.GenerateUploadURLAsync();
            return Ok(new { url });
        }
    }
}

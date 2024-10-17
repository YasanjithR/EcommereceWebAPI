using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
//this controller is responsible for handling notification related operations
namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        [Route("GetNotifications")]
        public async Task<IList<Notification>> GetNotifications()
        {
            // Retrieve the user ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Return an empty list if the user ID is null or empty
                return new List<Notification>();
            }

            // Get the notifications for the specified user
            var result = await _notificationService.GetNotifications(userId);
            return result;
        }

    }
}

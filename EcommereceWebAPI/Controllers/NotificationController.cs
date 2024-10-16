using EcommereceWebAPI.Data.Models;
using EcommereceWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommereceWebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NotificationController:ControllerBase
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new List<Notification>();
            }
            var result = await _notificationService.GetNotifications(userId);
            return result;
        }

    }
}

using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EcommereceWebAPI.Services
{
    public class NotificationService
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationService(MongoDbContext context)
        {
            _context = context;
            _notifications = _context.GetCollection<Notification>("Notification");
        }


        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            if(notification == null)
            {
                return new NotFoundObjectResult(new { message = "Notification null" });
            }

            await _notifications.InsertOneAsync(notification);

            return new OkObjectResult(new { message = "Notification service" });
        }
    }
}

using EcommereceWebAPI.Data;
using EcommereceWebAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

//this service is responsible for creating and retrieving notifications
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

        // Creates a new notification.
        /// <summary>
        /// Creates a new notification.
        /// </summary>
        /// <param name="notification">The notification to be created.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            if (notification == null)
            {
                return new NotFoundObjectResult(new { message = "Notification null" });
            }

            await _notifications.InsertOneAsync(notification);

            return new OkObjectResult(new { message = "Notification service" });
        }

        // Retrieves a list of notifications for the specified user.
        /// <summary>
        /// Retrieves a list of notifications for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of notifications.</returns>
        public async Task<IList<Notification>> GetNotifications(string userId)
        {
            try
            {
                var notifications = await _notifications.Find(n => n.UserId == userId).ToListAsync();

                if (notifications == null)
                {
                    return new List<Notification>();
                }

                return notifications;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

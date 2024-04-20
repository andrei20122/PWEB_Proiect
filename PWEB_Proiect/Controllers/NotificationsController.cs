using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEB_Proiect.DTOs;

namespace PWEB_Proiect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("get_notifications")]
        public async Task<ActionResult> GetNotifications([FromBody] NotificationsRequest request)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return NotFound(new ErrorMessageDTO { Error = "User not found" });
            }

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == user.Id && request.Types.Contains(n.Type))
                .Select(n => new
                {
                    n.Id,
                    CreatedTime = n.CreatedTime.ToString("o"), // ISO 8601 format
                    SenderUsername = n.Sender.Username, // Assuming 'Sender' is a navigation property to 'User'
                    ReceiverId = n.ReceiverId,
                    n.Content,
                    n.Type // Assuming 'Type' is stored as an integer
                })
                .ToListAsync();

            var filteredNotifications = notifications
                .Where(n => request.Types.Contains(n.Type)) // Filter by types
                .ToList();

            return Ok(new { Notifications = filteredNotifications });
        }

        
        [HttpPost("create_notification")]
        public async Task<ActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
        {

            // Find the sender's user ID based on the username
            var senderUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (senderUser == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Find the receiver's user ID based on the receiver username
            var receiverUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == dto.Receiver);
            if (receiverUser == null)
            {
                return NotFound(new { error = "Receiver user not found" });
            }

            // Create the notification
            var notification = new Notifications
            {
                CreatedTime = DateTime.UtcNow,
                SenderId = senderUser.Id,
                ReceiverId = receiverUser.Id,
                Content = dto.Content,
                Type = dto.Type
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notification created successfully", notification_id = notification.Id });
        }


    }
}

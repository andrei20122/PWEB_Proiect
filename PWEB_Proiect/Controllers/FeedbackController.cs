using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWEB_Proiect.DTOs;
using PWEB_Proiect.Entities;
using System.Security.Claims;

namespace PWEB_Proiect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("create_feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDTO request)
        {
            var username = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Ok(new ErrorMessageDTO() { Error = "Invalid data" });
            }

            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Subject = request.Subject,
                IsSatisfied = request.IsSatisfied,
                Features = request.Features,
                Comments = request.Comments,
                CreatedTime = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedback inserted successfully." });
        }

        /*[Authorize]
        [HttpGet("get_username_feedbacks")]
        public async Task<IActionResult> GetFeedbacks(string username)
        {
            var usernameFromToken = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            username = usernameFromToken;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new ErrorMessageDTO { Error = "User not found" });
            }

            var feedback = user.Feedback.Select(f => new
            {

                Subject = f.Subject,
                IsSatisfied = f.IsSatisfied,
                Features = f.Features,
                Comments = f.Comments,
                CreationDate = f.CreatedTime // Presupunând că există o astfel de proprietate
            });

            return Ok(feedback);
        }*/

        [Authorize(Roles ="admin")]
        [HttpGet("get_all_feedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacksWithUser = await _context.Feedbacks
                .Include(f => f.User) // Presupunând că ai o proprietate de navigare `User` în entitatea `Feedback`
                .Select(f => new
                {
                    Id = f.Id,
                    Username = f.User!.Username, // Accesează numele de utilizator prin entitatea de navigare
                    Subject = f.Subject,
                    IsSatisfied = f.IsSatisfied,
                    Features = f.Features,
                    Comments = f.Comments,
                    CreationDate = f.CreatedTime // Presupunând că există o astfel de proprietate
                })
                .ToListAsync();

            return Ok(feedbacksWithUser);
        }


        [Authorize(Roles ="admin")]
        [HttpDelete("delete_feedbacks_user")]
        public async Task<IActionResult> DeleteFeedback(string username)
        {
            var usernameFromToken = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name, null)?.Value;
            if (username == null)
                return Ok(new ErrorMessageDTO() { Error = "No username available" });

            username = usernameFromToken;
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Ok(new ErrorMessageDTO() { Error = "User not found" });
            }

            _context.Feedbacks.RemoveRange(user.Feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Feedbacks deleted successfully." });
        }



        /*[HttpDelete("delete_specific_feedback/{username}/{feedbackId}")]
        public async Task<IActionResult> DeleteSpecificFeedback(string username, Guid feedbackId)
        {
            var user = await _context.Users.Include(u => u.Feedbacks).SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new ErrorMessageDTO { Error = "User not found" });
            }

            var feedback = user.Feedbacks.FirstOrDefault(f => f.Id == feedbackId);
            if (feedback == null)
            {
                return NotFound(new ErrorMessageDTO { Error = "Feedback not found" });
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Specific feedback deleted successfully." });
        }*/

    }
}

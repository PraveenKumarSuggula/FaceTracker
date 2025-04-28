using facetrackr_backend.Data;
using facetrackr_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public FeedbackController(AttendanceContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public IActionResult SubmitFeedback([FromBody] Feedback feedback)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            feedback.UserId = userId;
            feedback.Date = DateTime.UtcNow;

            _context.Feedbacks.Add(feedback);
            _context.SaveChanges();
            return Ok(new { message = "Feedback submitted." });
        }
    }
}

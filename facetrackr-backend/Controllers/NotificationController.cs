using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Teacher")]
    public class NotificationController : ControllerBase
    {
        [HttpPost("send")]
        public IActionResult SendNotification()
        {
            // Placeholder for EmailJS integration (handled by frontend)
            return Ok(new { message = "Notification sent (placeholder)." });
        }
    }
}

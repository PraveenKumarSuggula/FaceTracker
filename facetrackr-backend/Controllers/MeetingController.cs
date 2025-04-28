using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Teacher, Admin")]
    public class MeetingController : ControllerBase
    {
        [HttpPost("start")]
        public IActionResult StartMeeting()
        {
            string meetingId = Guid.NewGuid().ToString();
            return Ok(new { meetingId });
        }

        [HttpPost("end")]
        public IActionResult EndMeeting([FromQuery] string meetingId)
        {
            // Optionally handle session cleanup
            return Ok(new { message = "Meeting ended", meetingId });
        }
    }
}

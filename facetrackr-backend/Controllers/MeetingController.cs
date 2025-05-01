using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MeetingController : ControllerBase
{
    private static Dictionary<string, bool> ActiveMeetings = new();

    [HttpGet("generate")]
    public IActionResult GenerateMeetingId()
    {
        var meetingId = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        ActiveMeetings[meetingId] = false;
        return Ok(new { meetingId });
    }

    [HttpPost("start")]
    public IActionResult StartMeeting([FromBody] MeetingRequest req)
    {
        ActiveMeetings[req.MeetingId] = true;
        return Ok(new { message = "Meeting started." });
    }

    [HttpPost("end")]
    public IActionResult EndMeeting([FromBody] MeetingRequest req)
    {
        if (ActiveMeetings.ContainsKey(req.MeetingId))
        {
            ActiveMeetings.Remove(req.MeetingId);
            return Ok(new { message = "Meeting ended." });
        }
        return NotFound();
    }

    [HttpGet("status/{meetingId}")]
    public IActionResult GetStatus(string meetingId)
    {
        return Ok(new { isActive = ActiveMeetings.TryGetValue(meetingId, out bool active) && active });
    }

    public class MeetingRequest
    {
        public string MeetingId { get; set; }
    }
}

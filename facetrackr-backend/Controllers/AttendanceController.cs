using facetrackr_backend.Data;
using facetrackr_backend.Models;
using facetrackr_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Teacher, Admin")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceContext _context;
        private readonly FaceRecognitionService _faceService;

        public AttendanceController(AttendanceContext context, FaceRecognitionService faceService)
        {
            _context = context;
            _faceService = faceService;
        }

        [HttpPost("mark")]
        public IActionResult MarkAttendance([FromBody] FaceInputModel input)
        {
            var faceMat = _faceService.ConvertBase64ToMat(input.FaceBase64);
            int userId = _faceService.Recognize(faceMat);

            if (userId == -1)
                return NotFound("Face not recognized.");

            var imagePath = _faceService.SaveFaceImage(faceMat, userId);

            var record = new AttendanceRecord
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Status = "Present",
                ImagePath = imagePath   // Add ImagePath in AttendanceRecord model
            };
            _context.AttendanceRecords.Add(record);
            _context.SaveChanges();

            return Ok(new { message = "Attendance marked.", userId });
        }

    }

    public class FaceInputModel
    {
        public string FaceBase64 { get; set; }
    }
}

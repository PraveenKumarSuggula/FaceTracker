using facetrackr_backend.Data;
using facetrackr_backend.Models;
using facetrackr_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student, Teacher, Admin")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceContext _context;
        private readonly FaceRecognitionService _faceService;

        public AttendanceController(AttendanceContext context, FaceRecognitionService faceService)
        {
            _context = context;
            _faceService = faceService;
        }

        //[HttpPost("mark")]
        //public IActionResult MarkAttendance([FromBody] FaceInputModel input)
        //{
        //    var faceMat = _faceService.ConvertBase64ToMat(input.FaceBase64);
        //    int recognizedUserId = _faceService.Recognize(faceMat);
        //    var userId = recognizedUserId != -1 ? recognizedUserId : input.UserId; // Always track studentId
        //    var imagePath = _faceService.SaveFaceImage(faceMat, userId, input.MeetingId); // include meetingId
        //    var status = recognizedUserId != -1 ? "Present" : "Absent";

        //    var record = new AttendanceRecord
        //    {
        //        UserId = userId,
        //        MeetingId = input.MeetingId,
        //        Date = DateTime.UtcNow,
        //        Status = status,
        //        ImagePath = imagePath
        //    };
        //    _context.AttendanceRecords.Add(record);
        //    _context.SaveChanges();

        //    return Ok(new { message = $"Marked {status}", imagePath });
        //}

        [HttpPost("mark")]
        public async Task<IActionResult> MarkAttendance([FromBody] FaceInputModel input)
        {
            var faceMat = _faceService.ConvertBase64ToMat(input.FaceBase64);
            var faceRects = _faceService.DetectFaces(faceMat);

            if (faceRects.Length == 0)
            {
                var path = _faceService.SaveFaceImage(faceMat, input.UserId, input.MeetingId, "Absent");

                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    UserId = input.UserId,
                    MeetingId = input.MeetingId,
                    Date = DateTime.UtcNow,
                    Status = "Absent",
                    ImagePath = path
                });
                _context.SaveChanges();

                return Ok(new { message = "No face detected. Marked Absent.", confidence = 0, path });
            }

            var cropped = new Mat(faceMat, faceRects[0]);

            var (isSimilar, label, confidence) = _faceService.RecognizeRelaxed(cropped, input.UserId);
            var status = isSimilar ? "Present" : "Absent";
            var savePath = _faceService.SaveFaceImage(cropped, input.UserId, input.MeetingId, status);

            var matchedUser = _context.Users.FirstOrDefault(u => u.Id == label);
            var name = matchedUser?.Name ?? "Unknown";

            _context.AttendanceRecords.Add(new AttendanceRecord
            {
                UserId = input.UserId,
                MeetingId = input.MeetingId,
                Date = DateTime.UtcNow,
                Status = status,
                ImagePath = savePath
            });
            _context.SaveChanges();

            return Ok(new { message = $"Marked {status}", label, name, confidence, savePath });
        }

        [HttpGet("report/{teacherId}")]
        public IActionResult GenerateReport(int teacherId)
        {
            var meetingDir = Path.Combine("FaceData", "CapturedFaces", teacherId.ToString());
            if (!Directory.Exists(meetingDir)) return NotFound("No images found.");

            var pdfPath = Path.Combine(meetingDir, $"AttendanceReport_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
            using (var doc = new PdfDocument())
            {
                foreach (var imgFile in Directory.GetFiles(meetingDir, "*.jpg"))
                {
                    var page = doc.AddPage();
                    using var xgr = XGraphics.FromPdfPage(page);
                    using var img = XImage.FromFile(imgFile);
                    xgr.DrawImage(img, 0, 0, page.Width, page.Height);
                }
                doc.Save(pdfPath);
            }

            var bytes = System.IO.File.ReadAllBytes(pdfPath);
            return File(bytes, "application/pdf", "Attendance_Report.pdf");
        }

        [HttpGet("report-by-meeting/{meetingId}")]
        public IActionResult GenerateReportByMeeting(string meetingId)
        {
            var students = _context.AttendanceRecords
                .Where(r => r.MeetingId == meetingId)
                .GroupBy(r => r.UserId)
                .ToList();

            string basePath = Path.Combine("FaceData", "CapturedFaces", meetingId);
            var pdfPath = Path.Combine(basePath, $"Report_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
            Directory.CreateDirectory(basePath);

            using (var doc = new PdfDocument())
            {
                foreach (var group in students)
                {
                    var total = group.Count();
                    var present = group.Count(x => x.Status == "Present");
                    var percentage = (present * 100) / total;

                    // Add stats page
                    var statsPage = doc.AddPage();
                    using (var g = XGraphics.FromPdfPage(statsPage))
                    {
                        g.DrawString($"User ID: {group.Key}", new XFont("Arial", 14), XBrushes.Black, 20, 40);
                        g.DrawString($"Present: {present} / {total} ({percentage}%)", new XFont("Arial", 12), XBrushes.Black, 20, 70);
                    }

                    // Add face images
                    foreach (var record in group)
                    {
                        var imgPath = record.ImagePath;
                        if (System.IO.File.Exists(imgPath))
                        {
                            var page = doc.AddPage();
                            using var xgr = XGraphics.FromPdfPage(page);
                            using var img = XImage.FromFile(imgPath);
                            xgr.DrawImage(img, 0, 0, page.Width, page.Height);
                        }
                    }
                }
                doc.Save(pdfPath);
            }

            var bytes = System.IO.File.ReadAllBytes(pdfPath);
            return File(bytes, "application/pdf", "Attendance_Report.pdf");
        }



    }

    public class FaceInputModel
    {
        public string FaceBase64 { get; set; }
        public int UserId { get; set; }
        public string MeetingId { get; set; }
    }
}

using facetrackr_backend.Data;
using facetrackr_backend.Models;
using facetrackr_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace facetrackr_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student, Teacher, Admin")]
    public class FaceRegistrationController : ControllerBase
    {
        private readonly FaceRecognitionService _faceService;

        public FaceRegistrationController(FaceRecognitionService faceService)
        {
            _faceService = faceService;
        }

        [HttpPost("register")]
        public IActionResult RegisterFace([FromBody] FaceRegisterInput input)
        {
            var faceMat = _faceService.ConvertBase64ToMat(input.FaceBase64);
            _faceService.RegisterFace(faceMat, input.UserId);

            return Ok(new { message = "Face registered successfully." });
        }

        [HttpGet("all")]
        [AllowAnonymous] // Or keep as [Authorize] based on access level
        public IActionResult GetAllDescriptors()
        {
            string registeredPath = Path.Combine(Directory.GetCurrentDirectory(), "FaceData", "RegisteredFaces");

            var result = new List<object>();
            foreach (var userFolder in Directory.GetDirectories(registeredPath))
            {
                var userId = Path.GetFileName(userFolder);
                var images = Directory.GetFiles(userFolder, "*.jpg");

                var descriptors = new List<float[]>();
                foreach (var image in images)
                {
                    byte[] imgBytes = System.IO.File.ReadAllBytes(image);
                    string base64 = Convert.ToBase64String(imgBytes);
                    descriptors.Add(new float[] { }); // Placeholder: skip if no embedding logic
                }

                result.Add(new { name = userId, descriptors });
            }

            return Ok(result);
        }

    }

    public class FaceRegisterInput
    {
        public int UserId { get; set; }
        public string FaceBase64 { get; set; }
    }
}

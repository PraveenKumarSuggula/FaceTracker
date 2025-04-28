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
    }

    public class FaceRegisterInput
    {
        public int UserId { get; set; }
        public string FaceBase64 { get; set; }
    }
}

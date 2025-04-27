namespace facetrackr_backend.Models
{
    public class UserLoginDto
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}

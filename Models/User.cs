namespace facetrackr_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }  // Admin, Teacher, Student
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
    }

}

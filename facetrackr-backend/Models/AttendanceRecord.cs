namespace facetrackr_backend.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }  // Present, Absent
    }

}

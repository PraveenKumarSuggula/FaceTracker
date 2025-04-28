using System.Text.Json.Serialization;

namespace facetrackr_backend.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

}

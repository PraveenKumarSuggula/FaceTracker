using Microsoft.EntityFrameworkCore;
using facetrackr_backend.Models;
using System.Collections.Generic;

namespace facetrackr_backend.Data
{
    public class AttendanceContext : DbContext
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Log> Logs { get; set; }
    }

}

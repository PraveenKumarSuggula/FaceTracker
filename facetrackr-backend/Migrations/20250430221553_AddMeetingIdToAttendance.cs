using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace facetrackr_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingIdToAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetingId",
                table: "AttendanceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "AttendanceRecords");
        }
    }
}

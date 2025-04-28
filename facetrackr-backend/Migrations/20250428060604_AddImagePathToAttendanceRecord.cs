using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace facetrackr_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AttendanceRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AttendanceRecords");
        }
    }
}

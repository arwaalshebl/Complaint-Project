using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintProj.Migrations
{
    /// <inheritdoc />
    public partial class StaffReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffReply",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffReply",
                table: "Complaints");
        }
    }
}

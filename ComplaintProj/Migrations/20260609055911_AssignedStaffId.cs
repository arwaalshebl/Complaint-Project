using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintProj.Migrations
{
    /// <inheritdoc />
    public partial class AssignedStaffId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedStaffId",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "Complaints");
        }
    }
}

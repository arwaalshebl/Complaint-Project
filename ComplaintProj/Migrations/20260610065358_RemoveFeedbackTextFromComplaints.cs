using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintProj.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFeedbackTextFromComplaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientFeedback",
                table: "Complaints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientFeedback",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

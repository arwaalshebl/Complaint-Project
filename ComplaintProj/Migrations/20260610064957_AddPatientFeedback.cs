using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintProj.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsSatisfied",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientFeedback",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSatisfied",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "PatientFeedback",
                table: "Complaints");
        }
    }
}

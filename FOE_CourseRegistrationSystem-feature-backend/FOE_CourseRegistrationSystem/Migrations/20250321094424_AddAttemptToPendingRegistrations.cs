using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemptToPendingRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRemarks",
                table: "PendingRegistrations");

            migrationBuilder.AddColumn<int>(
                name: "Attempt",
                table: "PendingRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IsApprovedByAdvisor",
                table: "PendingRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempt",
                table: "PendingRegistrations");

            migrationBuilder.DropColumn(
                name: "IsApprovedByAdvisor",
                table: "PendingRegistrations");

            migrationBuilder.AddColumn<string>(
                name: "AdminRemarks",
                table: "PendingRegistrations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseOfferingID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseOfferingID",
                table: "PendingRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingRegistrations_CourseOfferingID",
                table: "PendingRegistrations",
                column: "CourseOfferingID");

            migrationBuilder.AddForeignKey(
                name: "FK_PendingRegistrations_CourseOfferings_CourseOfferingID",
                table: "PendingRegistrations",
                column: "CourseOfferingID",
                principalTable: "CourseOfferings",
                principalColumn: "OfferingID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PendingRegistrations_CourseOfferings_CourseOfferingID",
                table: "PendingRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PendingRegistrations_CourseOfferingID",
                table: "PendingRegistrations");

            migrationBuilder.DropColumn(
                name: "CourseOfferingID",
                table: "PendingRegistrations");
        }
    }
}

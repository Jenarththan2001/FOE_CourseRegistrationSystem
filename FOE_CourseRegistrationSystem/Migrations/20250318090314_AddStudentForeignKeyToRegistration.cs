using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentForeignKeyToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_CourseOfferings_OfferingID",
                table: "Registrations");

            migrationBuilder.AddColumn<int>(
                name: "StudentID",
                table: "Registrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_StudentID",
                table: "Registrations",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_CourseOfferings_OfferingID",
                table: "Registrations",
                column: "OfferingID",
                principalTable: "CourseOfferings",
                principalColumn: "OfferingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Student_StudentID",
                table: "Registrations",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_CourseOfferings_OfferingID",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Student_StudentID",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_StudentID",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "StudentID",
                table: "Registrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_CourseOfferings_OfferingID",
                table: "Registrations",
                column: "OfferingID",
                principalTable: "CourseOfferings",
                principalColumn: "OfferingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

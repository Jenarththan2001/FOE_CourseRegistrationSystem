using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Staffs_AdviserID",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Student");

            migrationBuilder.RenameIndex(
                name: "IX_Students_AdviserID",
                table: "Student",
                newName: "IX_Student_AdviserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Staffs_AdviserID",
                table: "Student",
                column: "AdviserID",
                principalTable: "Staffs",
                principalColumn: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Staffs_AdviserID",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Students");

            migrationBuilder.RenameIndex(
                name: "IX_Student_AdviserID",
                table: "Students",
                newName: "IX_Students_AdviserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Staffs_AdviserID",
                table: "Students",
                column: "AdviserID",
                principalTable: "Staffs",
                principalColumn: "StaffID");
        }
    }
}

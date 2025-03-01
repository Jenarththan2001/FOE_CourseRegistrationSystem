using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Staffs_AdviserID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_AdviserID",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "AdviserID",
                table: "Student");

            migrationBuilder.RenameTable(
                name: "Staffs",
                newName: "Staff");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Student",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Staff",
                newName: "PasswordHash");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNo",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "Staffs");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Student",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Staffs",
                newName: "Password");

            migrationBuilder.AddColumn<int>(
                name: "AdviserID",
                table: "Student",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNo",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Student_AdviserID",
                table: "Student",
                column: "AdviserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Staffs_AdviserID",
                table: "Student",
                column: "AdviserID",
                principalTable: "Staffs",
                principalColumn: "StaffID");
        }
    }
}

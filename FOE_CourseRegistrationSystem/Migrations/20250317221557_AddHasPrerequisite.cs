using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddHasPrerequisite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HasPrerequisites",
                columns: table => new
                {
                    CourseCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrerequisiteCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HasPrerequisites", x => new { x.CourseCode, x.PrerequisiteCode });
                    table.ForeignKey(
                        name: "FK_HasPrerequisites_Course_CourseCode",
                        column: x => x.CourseCode,
                        principalTable: "Course",
                        principalColumn: "CourseCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HasPrerequisites_Course_PrerequisiteCode",
                        column: x => x.PrerequisiteCode,
                        principalTable: "Course",
                        principalColumn: "CourseCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HasPrerequisites_PrerequisiteCode",
                table: "HasPrerequisites",
                column: "PrerequisiteCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HasPrerequisites");
        }
    }
}

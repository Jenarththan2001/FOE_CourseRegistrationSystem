using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SenderMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ThirdPartyMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiverID = table.Column<int>(type: "int", nullable: false),
                    ReceiverType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SenderStudentID = table.Column<int>(type: "int", nullable: true),
                    SenderStaffID = table.Column<int>(type: "int", nullable: true),
                    SenderType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ThirdPartyStudentID = table.Column<int>(type: "int", nullable: true),
                    ThirdPartyStaffID = table.Column<int>(type: "int", nullable: true),
                    ThirdPartyType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsForAllStudents = table.Column<bool>(type: "bit", nullable: true),
                    AcademicYear = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    RelatedStudentID = table.Column<int>(type: "int", nullable: true),
                    RelatedStaffID = table.Column<int>(type: "int", nullable: true),
                    RelatedSessionID = table.Column<int>(type: "int", nullable: true),
                    RelatedPendingRegistrationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_PendingRegistrations_RelatedPendingRegistrationID",
                        column: x => x.RelatedPendingRegistrationID,
                        principalTable: "PendingRegistrations",
                        principalColumn: "PendingID");
                    table.ForeignKey(
                        name: "FK_Notification_RegistrationSessions_RelatedSessionID",
                        column: x => x.RelatedSessionID,
                        principalTable: "RegistrationSessions",
                        principalColumn: "SessionID");
                    table.ForeignKey(
                        name: "FK_Notification_Staff_RelatedStaffID",
                        column: x => x.RelatedStaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_Notification_Staff_SenderStaffID",
                        column: x => x.SenderStaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_Notification_Staff_ThirdPartyStaffID",
                        column: x => x.ThirdPartyStaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_Notification_Student_RelatedStudentID",
                        column: x => x.RelatedStudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                    table.ForeignKey(
                        name: "FK_Notification_Student_SenderStudentID",
                        column: x => x.SenderStudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                    table.ForeignKey(
                        name: "FK_Notification_Student_ThirdPartyStudentID",
                        column: x => x.ThirdPartyStudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RelatedPendingRegistrationID",
                table: "Notification",
                column: "RelatedPendingRegistrationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RelatedSessionID",
                table: "Notification",
                column: "RelatedSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RelatedStaffID",
                table: "Notification",
                column: "RelatedStaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RelatedStudentID",
                table: "Notification",
                column: "RelatedStudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderStaffID",
                table: "Notification",
                column: "SenderStaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderStudentID",
                table: "Notification",
                column: "SenderStudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ThirdPartyStaffID",
                table: "Notification",
                column: "ThirdPartyStaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ThirdPartyStudentID",
                table: "Notification",
                column: "ThirdPartyStudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using FOE_CourseRegistrationSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            try
            {
                // Using raw SQL query to avoid EF Core materialization issues with nulls
                var studentQuery = $"SELECT StudentID, Email, Password, FullName, AcademicYear, DepartmentID, StaffID FROM Student WHERE Email = '{model.Email}'";
                var staffQuery = $"SELECT StaffID, Email, Password, FullName, DepID, Role FROM Staff WHERE Email = '{model.Email}'";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = studentQuery;
                    await _context.Database.OpenConnectionAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                        {
                            var studentId = result.GetInt32(0);
                            var email = result.IsDBNull(1) ? null : result.GetString(1);
                            var password = result.IsDBNull(2) ? null : result.GetString(2);
                            var fullName = result.IsDBNull(3) ? "" : result.GetString(3);
                            var academicYear = result.IsDBNull(4) ? 0 : result.GetInt32(4);
                            var departmentId = result.IsDBNull(5) ? 0 : result.GetInt32(5);
                            var advisorId = result.IsDBNull(6) ? (int?)null : result.GetInt32(6);

                            if (!string.IsNullOrEmpty(password) && password == model.Password)
                            {
                                return Ok(new
                                {
                                    message = "Login successful",
                                    role = "Student",
                                    userId = studentId,
                                    fullName = fullName,
                                    academicYear = academicYear,
                                    departmentId = departmentId,
                                    advisorId = advisorId
                                });
                            }
                            else
                            {
                                return Unauthorized(new { message = "Invalid student credentials" });
                            }
                        }
                    }
                }

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = staffQuery;
                    if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        await _context.Database.OpenConnectionAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (await result.ReadAsync())
                        {
                            var staffId = result.GetInt32(0);
                            var email = result.IsDBNull(1) ? null : result.GetString(1);
                            var password = result.IsDBNull(2) ? null : result.GetString(2);
                            var fullName = result.IsDBNull(3) ? "" : result.GetString(3);
                            var depId = result.IsDBNull(4) ? 0 : result.GetInt32(4);
                            var roleValue = result.IsDBNull(5) ? 0 : result.GetInt32(5);
                            var role = (StaffRole)roleValue;

                            if (!string.IsNullOrEmpty(password) && password == model.Password)
                            {
                                return Ok(new
                                {
                                    message = "Login successful",
                                    role = role.ToString(),
                                    userId = staffId,
                                    fullName = fullName,
                                    departmentId = depId
                                });
                            }
                            else
                            {
                                return Unauthorized(new { message = "Invalid staff credentials" });
                            }
                        }
                    }
                }

                return Unauthorized(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
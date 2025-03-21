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
                // 🔹 Fetch student details while handling NULL values
                var student = await _context.Students
                    .Where(s => s.Email == model.Email && s.Password == model.Password)
                    .Select(s => new
                    {
                        s.StudentID,
                        s.FullName,
                        s.AcademicYear,
                        s.DepartmentID,
                        s.StaffID,
                        Email = s.Email ?? "",  // Handle NULL values
                        NIC = s.NIC ?? "",      // Handle NULL values
                        Nationality = s.Nationality ?? "",
                        Photo = s.Photo ?? "",
                        TempAddress = s.TempAddress ?? "",
                        PermanentAddress = s.PermanentAddress ?? "",
                        Gender = s.Gender ?? "",
                        PhoneNo = s.PhoneNo ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (student != null)
                {
                    return Ok(new
                    {
                        message = "Login successful",
                        role = "Student",
                        userId = student.StudentID,
                        fullName = student.FullName,
                        academicYear = student.AcademicYear,
                        departmentId = student.DepartmentID,
                        advisorId = student.StaffID
                    });
                }

                // 🔹 Fetch staff details while handling NULL values
                var staff = await _context.Staffs
                    .Where(st => st.Email == model.Email && st.Password == model.Password)
                    .Select(st => new
                    {
                        st.StaffID,
                        st.FullName,
                        st.DepID,
                        Role = st.Role.ToString(),
                        Email = st.Email ?? "",  // Handle NULL values
                        PhoneNo = st.PhoneNo ?? "" // Handle NULL values
                    })
                    .FirstOrDefaultAsync();

                if (staff != null)
                {
                    return Ok(new
                    {
                        message = "Login successful",
                        role = staff.Role,
                        userId = staff.StaffID,
                        fullName = staff.FullName,
                        departmentId = staff.DepID
                    });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}

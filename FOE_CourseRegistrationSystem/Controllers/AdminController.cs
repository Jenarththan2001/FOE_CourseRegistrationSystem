using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "AR")] // Only Admin can access
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads Course Offering Page
        /// </summary>
        public async Task<IActionResult> CourseOffering()
        {
            // ✅ Fetch Departments (Exclude Administration)
            ViewBag.Departments = await _context.Departments
                .Where(d => d.DepartmentID != 6) // Excluding Administration
                .ToListAsync();

            // ✅ Fetch Distinct Academic Years from Student Table
            ViewBag.AcademicYears = await _context.Students
                .Select(s => s.AcademicYear)
                .Distinct()
                .ToListAsync();

            // ✅ Fetch Courses (Include department details)
            ViewBag.Courses = await _context.Courses
                .Include(c => c.Department)
                .ToListAsync();

            return View("~/Views/Dashboard/Admin/CourseOffering.cshtml");
        }

        /// <summary>
        /// Handles Registration Session Creation
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRegistrationSession(
            [FromForm] string[] academicYears,
            [FromForm] int semester,
            [FromForm] int? departmentId,
            [FromForm] DateTime closingDate,
            [FromForm] List<string> selectedCourses)
        {
            if (academicYears.Length == 0 || semester == 0 || closingDate == default || selectedCourses.Count == 0)
            {
                return Json(new { success = false, message = "Invalid input. Please fill all required fields." });
            }

            // ✅ Auto-select General Program for Semesters 1-3
            bool isGeneralProgram = semester <= 3;
            departmentId = isGeneralProgram ? null : departmentId; // Set department to NULL for General Program

            foreach (var academicYear in academicYears)
            {
                // ✅ Check if registration session already exists
                var existingSession = await _context.RegistrationSessions
                    .FirstOrDefaultAsync(rs => rs.AcademicYear == academicYear && rs.Semester == semester.ToString());

                if (existingSession != null)
                {
                    return Json(new { success = false, message = $"Registration session already exists for {academicYear}, Semester {semester}." });
                }

                // ✅ Create New Registration Session
                var registrationSession = new RegistrationSession
                {
                    AcademicYear = academicYear,
                    Semester = semester.ToString(),
                    DepartmentID = departmentId,
                    StartDate = DateTime.UtcNow,
                    EndDate = closingDate,
                    IsGeneralProgram = isGeneralProgram,
                    IsOpen = true
                };

                _context.RegistrationSessions.Add(registrationSession);
                await _context.SaveChangesAsync(); // Get SessionID after saving

                // ✅ Add Selected Courses to the Session
                var sessionCourses = selectedCourses.Select(courseCode => new RegistrationSessionCourse
                {
                    SessionID = registrationSession.SessionID,
                    CourseCode = courseCode
                }).ToList();

                _context.RegistrationSessionCourses.AddRange(sessionCourses);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Registration session created successfully!" });
        }

        /// <summary>
        /// Loads Admin Dashboard
        /// </summary>
        public IActionResult AdminDashboard()
        {
            return View("~/Views/Dashboard/Admin/AdminDashboard.cshtml");
        }

        /// <summary>
        /// Loads Registration Details Page
        /// </summary>
        public IActionResult RegistrationDetails()
        {
            return View("~/Views/Dashboard/Admin/RegistrationDetails.cshtml");
        }

        /// <summary>
        /// Loads Advisor Details Page
        /// </summary>
        public IActionResult AdvisorDetails()
        {
            return View("~/Views/Dashboard/Admin/AdvisorDetails.cshtml");
        }
    }
}

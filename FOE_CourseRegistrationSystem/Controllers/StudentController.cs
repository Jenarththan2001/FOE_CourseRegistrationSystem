using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            string studentEmail = User.Identity.Name;

            // Fetch student details
            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Advisor)
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Fetch courses where results are available (Total Credits Earned)
            var results = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .ThenInclude(co => co.Course)
                .ToListAsync();

            int totalCreditsEarned = results.Sum(r => r.CourseOffering.Course.Credit); // Only courses with results
            int remainingCredits = 153 - totalCreditsEarned; // Engineering degree requires 153 credits

            // Calculate GPA
            double totalGradePoints = results.Sum(r => ConvertGradeToPoint(r.Grade) * r.CourseOffering.Course.Credit);
            double gpa = (totalCreditsEarned > 0) ? totalGradePoints / totalCreditsEarned : 0;

            // Pass data to view
            ViewData["Student"] = student;
            ViewData["GPA"] = gpa;
            ViewData["TotalCreditsEarned"] = totalCreditsEarned;
            ViewData["RemainingCredits"] = remainingCredits;

            return View("~/Views/Dashboard/Student/StudentDashboard.cshtml");
        }

        private double ConvertGradeToPoint(string grade)
        {
            return grade switch
            {
                "A+" => 4.0,
                "A" => 4.0,
                "A-" => 3.7,
                "B+" => 3.3,
                "B" => 3.0,
                "B-" => 2.7,
                "C+" => 2.3,
                "C" => 2.0,
                "C-" => 1.7,
                "D+" => 1.3,
                "D" => 1.0,
                "F" => 0.0,
                _ => 0.0
            };
        }


        public async Task<IActionResult> ResultPage()
        {
            string studentEmail = User.Identity.Name;

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Advisor)
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var results = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .ThenInclude(co => co.Course)
                .ToListAsync();

            ViewData["Student"] = student;
            ViewData["Results"] = results;

            return View("~/Views/Dashboard/Student/ResultPage.cshtml");
        }

        public IActionResult CourseRegistration()
        {
            return View("~/Views/Dashboard/Student/CourseRegistration.cshtml");
        }

        public async Task<IActionResult> RegisteredCourse()
        {
            string studentEmail = User.Identity.Name;

            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Advisor)
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var registeredCourses = await _context.Registrations
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(r => r.CourseOffering.Coordinator)
                .ToListAsync();

            // ✅ Fetch Prerequisites Separately
            var coursePrerequisites = await _context.HasPrerequisites
                .ToDictionaryAsync(p => p.CourseCode, p => p.PrerequisiteCode);

            ViewData["Student"] = student;
            ViewData["RegisteredCourses"] = registeredCourses;
            ViewData["CoursePrerequisites"] = coursePrerequisites;

            return View("~/Views/Dashboard/Student/RegisteredCourse.cshtml");
        }



        public IActionResult RegisterNewCourse()
        {
            return View("~/Views/Dashboard/Student/RegisterNewCourse.cshtml");
        }
    }
}

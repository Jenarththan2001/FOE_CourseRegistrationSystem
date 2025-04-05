using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SemesterService _semesterService;

        public CoordinatorController(ApplicationDbContext context, SemesterService semesterService)
        {
            _context = context;
            _semesterService = semesterService;
        }

        // ✅ Dashboard
        public async Task<IActionResult> CoordinatorDashboard()
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            if (coordinator == null) return NotFound();

            ViewBag.CoordinatorName = coordinator.FullName ?? "Unknown";
            ViewBag.CoordinatorPhone = coordinator.PhoneNo ?? "Unknown";
            ViewBag.CoordinatorEmail = coordinator.Email ?? "Unknown";

            int coordinatedCoursesCount = await _context.CourseOfferings
                .Where(co => co.StaffID == coordinator.StaffID)
                .CountAsync();
            ViewBag.CoordinatedCourseCount = coordinatedCoursesCount;

            var enrollments = await _context.CourseOfferings
                .Where(o => o.StaffID == coordinator.StaffID)
                .Include(o => o.Course)
                .GroupJoin(
                    _context.Registrations,
                    co => co.OfferingID,
                    r => r.OfferingID,
                    (co, registrations) => new
                    {
                        CourseName = co.Course.CourseName,
                        AcademicYear = co.AcademicID,
                        Semester = co.Semester,
                        StudentCount = registrations.Count()
                    })
                .ToListAsync();

            ViewBag.Enrollments = enrollments;
            return View("~/Views/Dashboard/Coordinator/CoordinatorDashboard.cshtml");
        }

        // ✅ Coordinating Courses
        public async Task<IActionResult> CoordinatingCourses()
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            ViewBag.CoordinatorName = coordinator?.FullName ?? "Unknown";
            return View("~/Views/Dashboard/Coordinator/CoordinatingCourses.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetCoordinatedCourses()
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            if (coordinator == null) return Unauthorized();

            var offerings = await _context.CourseOfferings
                .Where(o => o.StaffID == coordinator.StaffID)
                .Include(o => o.Course)
                .ToListAsync();

            var offeringIds = offerings.Select(o => o.OfferingID).ToList();

            var regCounts = await _context.Registrations
                .Where(r => offeringIds.Contains(r.OfferingID))
                .GroupBy(r => r.OfferingID)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync();

            var data = offerings.Select(o => new
            {
                o.CourseCode,
                CourseName = o.Course.CourseName,
                StudentCount = regCounts.FirstOrDefault(rc => rc.Key == o.OfferingID)?.Count ?? 0
            }).ToList();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsByCourse(string courseCode)
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            if (coordinator == null) return Unauthorized();

            var offeringIds = await _context.CourseOfferings
                .Where(o => o.CourseCode == courseCode && o.StaffID == coordinator.StaffID)
                .Select(o => o.OfferingID)
                .ToListAsync();

            var students = await _context.Registrations
                .Where(r => offeringIds.Contains(r.OfferingID))
                .Include(r => r.Student)
                .Select(r => new
                {
                    RegNo = r.Student.StudentID,
                    FullName = r.Student.FullName,
                    Email = r.Student.Email
                })
                .Distinct()
                .ToListAsync();

            return Json(students);
        }

        // ✅ View Advisee Students
        public async Task<IActionResult> AdviseeStudents()
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            if (coordinator == null) return Unauthorized();

            var students = await _context.Students
                .Where(s => s.StaffID == coordinator.StaffID)
                .Include(s => s.Department)
                .ToListAsync();

            var list = students.Select(s => new
            {
                s.StudentID,
                s.FullName,
                s.AcademicYear,
                CurrentSemester = _semesterService.GetCurrentSemester(s)
            }).ToList();

            ViewBag.AdvisorName = coordinator.FullName;
            ViewBag.Students = list;

            return View("~/Views/Dashboard/Coordinator/AdviseeStudents.cshtml");
        }
        public async Task<IActionResult> AdviseeStudentsResult(int studentId)
        {
            var email = User.Identity.Name;
            var coordinator = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (coordinator == null || student == null) return NotFound();

            ViewBag.AdvisorName = coordinator.FullName;
            ViewBag.StudentRegNo = student.StudentID.ToString();
            ViewBag.Role = "Coordinator";  // ✅ Add this line


            return View("~/Views/Dashboard/Coordinator/AdviseeStudentsResult.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentResults(int studentId)
        {
            var results = await _context.Results
                .Where(r => r.StudentID == studentId)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Select(r => new
                {
                    r.CourseOffering.Course.CourseCode,
                    r.CourseOffering.Course.CourseName,
                    r.CourseOffering.Semester,
                    r.InCourse,
                    r.EndMarks,
                    r.Grade,
                    Status = (r.InCourse + r.EndMarks) / 2.0 >= 50 ? "Pass" : "Fail"
                })
                .ToListAsync();

            return Json(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingRegistrations(int studentId)
        {
            var pendingCourses = await _context.PendingRegistrations
                .Where(pr => pr.StudentID == studentId && pr.Status == "Pending" && pr.IsApprovedByAdvisor == "No")
                .Include(pr => pr.RegistrationSessionCourse)
                    .ThenInclude(rsc => rsc.Course)
                .Include(pr => pr.RegistrationSessionCourse)
                    .ThenInclude(rsc => rsc.RegistrationSession)
                .Select(pr => new
                {
                    pr.CourseCode,
                    CourseName = pr.RegistrationSessionCourse.Course.CourseName,
                    Credits = pr.RegistrationSessionCourse.Course.Credit,
                    Semester = "Semester " + pr.RegistrationSessionCourse.Course.Semester,
                    Prerequisites = _context.HasPrerequisites
                        .Where(p => p.CourseCode == pr.CourseCode)
                        .Select(p => p.PrerequisiteCode)
                        .ToList()
                })
                .ToListAsync();

            return Json(pendingCourses);
        }

    }
}

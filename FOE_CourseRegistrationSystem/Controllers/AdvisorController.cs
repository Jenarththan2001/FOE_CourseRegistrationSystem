using FOE_CourseRegistrationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Make sure you have this namespace
using System.Text.Json;
using FOE_CourseRegistrationSystem.Services;


namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Advisor")]
    public class AdvisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SemesterService _semesterService;

        public AdvisorController(ApplicationDbContext context, SemesterService semesterService)
        {
            _context = context;
            _semesterService = semesterService;
        }

        public async Task<IActionResult> AdviserDashboard()
        {
            var advisorEmail = User.Identity.Name;
            var advisor = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == advisorEmail);
            int adviseeCount = await _context.Students.CountAsync(s => s.StaffID == advisor.StaffID);
            // ✅ Group advisees by AcademicYear and get count
            var adviseeGroups = await _context.Students
                .Where(s => s.StaffID == advisor.StaffID)
                .GroupBy(s => s.AcademicYear)
                .Select(g => new
                {
                    AcademicYear = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // ✅ Fetch advisees
            var adviseeIds = await _context.Students
                .Where(s => s.StaffID == advisor.StaffID)
                .Select(s => s.StudentID)
                .ToListAsync();

            // ✅ Count pending requests for these advisees
            int pendingRequestsCount = await _context.PendingRegistrations
                .Where(p => adviseeIds.Contains(p.StudentID) && p.Status == "Pending" && p.IsApprovedByAdvisor == "No")
                .CountAsync();

            ViewBag.PendingRequests = pendingRequestsCount;


            ViewBag.AdvisorName = advisor?.FullName ?? "Unknown";
            ViewBag.AdvisorEmail = advisor?.Email ?? "Unknown";
            ViewBag.AdvisorPhone = advisor?.PhoneNo ?? "Unknown";
            ViewBag.AdviseeCount = adviseeCount;  // <-- pass advisee count
            ViewBag.AdviseeGroups = adviseeGroups; // passing grouped data

            return View("~/Views/Dashboard/Advisor/AdviserDashboard.cshtml");
        }


        public async Task<IActionResult> AdviseeStudents()
        {
            string advisorEmail = User.Identity.Name;
            var advisor = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == advisorEmail);

            if (advisor == null)
            {
                return Unauthorized("Advisor not found.");
            }

            // Include department if needed by the semester service
            var students = await _context.Students
                .Where(s => s.StaffID == advisor.StaffID)
                .Include(s => s.Department)
                .ToListAsync();

            // Map with current semester
            var studentList = students.Select(s => new
            {
                s.StudentID,
                s.FullName,
                s.AcademicYear,
                CurrentSemester = _semesterService.GetCurrentSemester(s)
            }).ToList();

            ViewBag.AdvisorName = advisor?.FullName ?? "Unknown"; // 👈 Added
            ViewBag.Students = studentList;
            return View("~/Views/Dashboard/Advisor/AdviseeStudents.cshtml");

        }


        public async Task<IActionResult> AdvisorNotification()
        {
            var advisorEmail = User.Identity.Name;
            var advisor = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == advisorEmail);
            ViewBag.AdvisorName = advisor?.FullName ?? "Unknown";
            return View("~/Views/Dashboard/Advisor/AdvisorNotification.cshtml");
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

        [HttpPost]
        public async Task<IActionResult> SubmitApprovals([FromBody] JsonElement data)
        {
            try
            {
                // Extract studentId
                int studentId = int.Parse(data.GetProperty("studentId").GetString());

                // Extract and iterate over courses
                var courses = data.GetProperty("courses").EnumerateArray();
                foreach (var course in courses)
                {
                    string courseCode = course.GetProperty("courseCode").GetString();
                    string status = course.GetProperty("status").GetString();

                    // Find the pending registration
                    var pending = await _context.PendingRegistrations
                        .FirstOrDefaultAsync(p => p.StudentID == studentId
                            && p.CourseCode == courseCode
                            && p.Status == "Pending");

                    if (pending != null)
                    {
                        if (status == "Approved")
                        {
                            pending.IsApprovedByAdvisor = "Yes";
                        }
                        else if (status == "Rejected")
                        {
                            pending.Status = "Rejected";
                            // Leave IsApprovedByAdvisor = "No"
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // ✅ Return success flag for JavaScript
                return Json(new { success = true, message = "Approvals processed successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in SubmitApprovals: " + ex.Message);
                return Json(new { success = false, message = "Error processing approvals", error = ex.Message });
            }
        }






    }
}

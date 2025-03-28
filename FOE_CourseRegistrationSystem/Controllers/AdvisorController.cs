using FOE_CourseRegistrationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Make sure you have this namespace
using System.Text.Json;


namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Advisor")]
    public class AdvisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdviserDashboard()
        {
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

            var students = await _context.Students
                .Where(s => s.StaffID == advisor.StaffID)
                .Select(s => new
                {
                    s.StudentID,
                    s.FullName,
                    s.AcademicYear
                })
                .ToListAsync();

            ViewBag.Students = students;
            return View("~/Views/Dashboard/Advisor/AdviseeStudents.cshtml");
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

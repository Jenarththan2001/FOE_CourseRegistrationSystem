using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FOE_CourseRegistrationSystem.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coordinator Dashboard
        public IActionResult CoordinatorDashboard()
        {
            return View("~/Views/Dashboard/Coordinator/CoordinatorDashboard.cshtml");
        }

        // GET: Coordinating Courses Page
        public IActionResult CoordinatingCourses()
        {
            return View("~/Views/Dashboard/Coordinator/CoordinatingCourses.cshtml");
        }


        // GET: API endpoint to fetch courses coordinated by the logged-in coordinator along with student count
        [HttpGet]
        public async Task<IActionResult> GetCoordinatedCourses()
        {
            string coordinatorEmail = User.Identity.Name;

            var coordinator = await _context.Staffs
                .FirstOrDefaultAsync(s => s.Email == coordinatorEmail);

            if (coordinator == null)
                return Unauthorized("Coordinator not found.");

            // Get all offerings for this coordinator
            var courseOfferings = await _context.CourseOfferings
                .Where(o => o.StaffID == coordinator.StaffID)
                .Include(o => o.Course)
                .ToListAsync();

            var offeringIds = courseOfferings.Select(o => o.OfferingID).ToList();

            // Count registered students per offering
            var registrationCounts = await _context.Registrations
                .Where(r => offeringIds.Contains(r.OfferingID))
                .GroupBy(r => r.OfferingID)
                .Select(g => new
                {
                    OfferingID = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Combine course + student count per offering
            var courseData = courseOfferings.Select(o => new
            {
                o.CourseCode,
                CourseName = o.Course.CourseName,
                StudentCount = registrationCounts
                    .FirstOrDefault(rc => rc.OfferingID == o.OfferingID)?.Count ?? 0
            }).ToList();

            return Json(courseData);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsByCourse(string courseCode)
        {
            string coordinatorEmail = User.Identity.Name;

            var coordinator = await _context.Staffs
                .FirstOrDefaultAsync(s => s.Email == coordinatorEmail);

            if (coordinator == null)
                return Unauthorized("Coordinator not found.");

            // Get the offering(s) of this course by this coordinator
            var offerings = await _context.CourseOfferings
                .Where(o => o.CourseCode == courseCode && o.StaffID == coordinator.StaffID)
                .ToListAsync();

            var offeringIds = offerings.Select(o => o.OfferingID).ToList();

            // Get students registered for those offerings
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

    }
}

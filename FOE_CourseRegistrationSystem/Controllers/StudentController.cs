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


        [HttpGet]
        public async Task<IActionResult> GetAvailableCourses()
        {
            Console.WriteLine("🔹 API Called: GetAvailableCourses() Running...");

            string studentEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(studentEmail))
            {
                Console.WriteLine("❌ No Student Email Found in Session.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            Console.WriteLine($"✅ Student Email Found: {studentEmail}");

            // Fetch student details
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                Console.WriteLine("❌ Student not found in the database.");
                return NotFound(new { message = "Student not found." });
            }

            Console.WriteLine($"✅ Student Found: {student.FullName} | Academic Year: {student.AcademicYear} | DepartmentID: {student.DepartmentID}");

            // Fetch open registration sessions for the student
            var openSessions = await _context.RegistrationSessions
                .Where(rs => rs.AcademicYear == student.AcademicYear && rs.IsOpen && rs.EndDate >= DateTime.UtcNow)
                .Include(rs => rs.RegistrationSessionCourses)
                .ThenInclude(rsc => rsc.Course)
                .ToListAsync();

            if (!openSessions.Any())
            {
                Console.WriteLine("❌ No open registration sessions found.");
                return NotFound(new { message = "No open registration sessions available for your academic year." });
            }

            Console.WriteLine($"✅ Found {openSessions.Count} open registration sessions.");

            // ✅ Get student's past results with course codes and average marks
            var pastResults = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .Select(r => new
                {
                    CourseCode = r.CourseOffering.CourseCode,
                    Average = (r.InCourse + r.EndMarks) / 2.0
                })
                .ToListAsync();

            // ✅ Courses that student has passed already
            var passedCourses = pastResults
                .Where(r => r.Average >= 50)
                .Select(r => r.CourseCode)
                .Distinct()
                .ToList();

            // ✅ Count the number of attempts for each course
            var pastAttempts = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .GroupBy(r => r.CourseOffering.CourseCode)
                .Select(g => new { CourseCode = g.Key, AttemptCount = g.Count() })
                .ToDictionaryAsync(x => x.CourseCode, x => x.AttemptCount);

            // ✅ Courses already in pending registration
            var alreadyRegisteredCourses = await _context.PendingRegistrations
                .Where(pr => pr.StudentID == student.StudentID)
                .Select(pr => pr.CourseCode)
                .ToListAsync();

            Console.WriteLine($"✅ Passed Courses: {string.Join(", ", passedCourses)}");
            Console.WriteLine($"✅ Pending Registrations: {string.Join(", ", alreadyRegisteredCourses)}");

            // ✅ Fetch available courses with filtering
            var availableCourses = openSessions
                .Where(rs => rs.IsGeneralProgram || rs.DepartmentID == student.DepartmentID)
                .SelectMany(rs => rs.RegistrationSessionCourses
                    .Where(rsc =>
                        !alreadyRegisteredCourses.Contains(rsc.Course.CourseCode) &&  // Not in pending
                        (!passedCourses.Contains(rsc.Course.CourseCode) ||  // If failed, allow reattempt
                         pastResults.Any(r => r.CourseCode == rsc.Course.CourseCode && r.Average < 50))
                    )
                    .Select(rsc => new
                    {
                        rsc.Course.CourseCode,
                        rsc.Course.CourseName,
                        rsc.Course.Credit,
                        Semester = rs.Semester,
                        ClosingDate = rs.EndDate.ToString("yyyy-MM-dd"),
                        Coordinator = "Not Assigned",
                        Attempt = pastAttempts.ContainsKey(rsc.Course.CourseCode)
                            ? pastAttempts[rsc.Course.CourseCode] + 1  // Increment attempt count
                            : 1, // First attempt
                        Prerequisites = _context.HasPrerequisites
                            .Where(hp => hp.CourseCode == rsc.Course.CourseCode)
                            .Select(hp => hp.PrerequisiteCode)
                            .ToList()
                    }))
                .ToList();

            // ✅ Unique Semesters for Dropdown
            var uniqueSemesters = availableCourses
                .Select(c => c.Semester)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            Console.WriteLine($"✅ Returning {availableCourses.Count} filtered courses.");
            Console.WriteLine($"✅ Available Semesters: {string.Join(", ", uniqueSemesters)}");

            return Json(new { courses = availableCourses, semesters = uniqueSemesters });
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


        public async Task<IActionResult> RegisterNewCourse()
        {
            string studentEmail = User.Identity.Name;

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            ViewData["Student"] = student; // ✅ Ensures data is available in View
            return View("~/Views/Dashboard/Student/RegisterNewCourse.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitCourseRegistration([FromBody] List<string> selectedCourses)
        {
            string studentEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            Console.WriteLine($"✅ Student {student.StudentID} is registering for {selectedCourses.Count} courses.");

            // Fetch valid courses that exist in the RegistrationSessionCourses table
            var validCourses = await _context.RegistrationSessionCourses
                .Where(rsc => selectedCourses.Contains(rsc.CourseCode))
                .ToListAsync();

            if (!validCourses.Any())
            {
                return BadRequest(new { message = "Selected courses are not part of the current session." });
            }

            // ✅ Check for duplicate registrations
            var existingRegistrations = await _context.PendingRegistrations
                .Where(pr => pr.StudentID == student.StudentID && selectedCourses.Contains(pr.CourseCode))
                .Select(pr => pr.CourseCode)
                .ToListAsync();

            var newCourses = validCourses
                .Where(vc => !existingRegistrations.Contains(vc.CourseCode))
                .ToList();

            if (!newCourses.Any())
            {
                return BadRequest(new { message = "All selected courses have already been submitted for registration." });
            }

            // ✅ Step 1: Fetch all prerequisites for selected courses
            var prerequisites = await _context.HasPrerequisites
                .Where(hp => selectedCourses.Contains(hp.CourseCode))
                .ToListAsync();

            // ✅ Step 2: Fetch student's results for prerequisite courses
            var studentResults = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .ToListAsync();

            // ✅ Step 3: Validate Prerequisites
            List<string> failedCourses = new List<string>();

            foreach (var prereq in prerequisites)
            {
                var result = studentResults.FirstOrDefault(r => r.CourseOffering != null && r.CourseOffering.CourseCode == prereq.PrerequisiteCode);

                if (result != null)
                {
                    double averageMarks = (result.InCourse + result.EndMarks) / 2.0;
                    Console.WriteLine($"➡ Checking prerequisite {prereq.PrerequisiteCode} for {prereq.CourseCode}: Marks = {averageMarks}");

                    if (averageMarks < 50)
                    {
                        failedCourses.Add(prereq.CourseCode);
                    }
                }
                else
                {
                    Console.WriteLine($"⚠ Prerequisite {prereq.PrerequisiteCode} has no results released. Allowing registration.");
                }
            }

            if (failedCourses.Any())
            {
                return BadRequest(new
                {
                    message = "You have failed prerequisite courses required for registration.",
                    failedCourses
                });
            }

            // ✅ Step 4: Insert into PendingRegistration table
            var pendingRegistrations = newCourses.Select(course => new PendingRegistration
            {
                SessionID = course.SessionID,
                CourseCode = course.CourseCode,
                StudentID = student.StudentID,
                Status = "Pending",
                RegistrationDate = DateTime.UtcNow,
                AdminRemarks = "" // ✅ Set empty string if NULL is not allowed
            }).ToList();

            _context.PendingRegistrations.AddRange(pendingRegistrations);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ {pendingRegistrations.Count} courses submitted for student {student.StudentID}.");

            return Ok(new { message = "Course registration submitted successfully!" });
        }







    }
}

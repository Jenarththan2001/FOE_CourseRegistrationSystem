using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using FOE_CourseRegistrationSystem.Services;

namespace FOE_CourseRegistrationSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SemesterService _semesterService;
        private readonly CreditTrackingService _creditTrackingService;

        public StudentController(ApplicationDbContext context, SemesterService semesterService, CreditTrackingService creditTrackingService)
        {
            _context = context;
            _semesterService = semesterService;
            _creditTrackingService = creditTrackingService;
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

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == studentEmail);

            if (student == null)
            {
                Console.WriteLine("❌ Student not found in the database.");
                return NotFound(new { message = "Student not found." });
            }

            Console.WriteLine($"✅ Student Found: {student.FullName} | Academic Year: {student.AcademicYear} | DepartmentID: {student.DepartmentID}");

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

            Console.WriteLine("✅ ✅ ✅ Available Open Sessions:");
            foreach (var session in openSessions)
            {
                Console.WriteLine($"   - SessionID: {session.SessionID}, Semester: {session.Semester}, EndDate: {session.EndDate:yyyy-MM-dd}, Courses: {session.RegistrationSessionCourses.Count}");
            }

            Console.WriteLine($"✅ Found {openSessions.Count} open registration sessions.");

            var pastResults = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .Select(r => new
                {
                    CourseCode = r.CourseOffering.CourseCode,
                    Average = (r.InCourse + r.EndMarks) / 2.0
                })
                .ToListAsync();

            var passedCourses = pastResults
                .Where(r => r.Average >= 50)
                .Select(r => r.CourseCode)
                .Distinct()
                .ToList();

            var pastAttempts = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .GroupBy(r => r.CourseOffering.CourseCode)
                .Select(g => new { CourseCode = g.Key, AttemptCount = g.Count() })
                .ToDictionaryAsync(x => x.CourseCode, x => x.AttemptCount);

            var openSessionIDs = openSessions.Select(s => s.SessionID).ToList();
            // ✅ Only exclude if the course is already Approved (not just pending)
            var alreadyApprovedCourses = await _context.PendingRegistrations
               .Where(pr => pr.StudentID == student.StudentID && pr.Status == "Approved" && openSessionIDs.Contains(pr.SessionID))
                .Select(pr => pr.CourseCode)
                .ToListAsync();

            Console.WriteLine($"✅ Passed Courses: {string.Join(", ", passedCourses)}");
            Console.WriteLine($"✅ Already Approved Courses: {string.Join(", ", alreadyApprovedCourses)}");

            // ✅ ✅ ✅  ADD CourseOfferingID in result
            var availableCourses = openSessions
                .Where(rs => rs.IsGeneralProgram || rs.DepartmentID == student.DepartmentID)
                .SelectMany(rs => rs.RegistrationSessionCourses
                    .Where(rsc =>
                        !alreadyApprovedCourses.Contains(rsc.Course.CourseCode) &&   // Only approved should be blocked
                        (!passedCourses.Contains(rsc.Course.CourseCode) ||           // If failed, allow retry
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
                        Attempt = pastAttempts.ContainsKey(rsc.Course.CourseCode) ? pastAttempts[rsc.Course.CourseCode] + 1 : 1,
                        Prerequisites = _context.HasPrerequisites
                            .Where(hp => hp.CourseCode == rsc.Course.CourseCode)
                            .Select(hp => hp.PrerequisiteCode)
                            .ToList(),

                        CourseOfferingID = _context.CourseOfferings
                            .Where(co => co.CourseCode == rsc.Course.CourseCode &&
                                         co.AcademicID == student.AcademicYear &&
                                         co.Semester == rs.Semester)
                            .Select(co => co.OfferingID)
                            .FirstOrDefault()
                    }))
                .ToList();

            var uniqueSemesters = availableCourses
                .Select(c => c.Semester)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            Console.WriteLine($"✅ Returning {availableCourses.Count} filtered courses.");
            Console.WriteLine($"✅ Available Semesters (after filtering): {string.Join(", ", uniqueSemesters)}");

            return Json(new { courses = availableCourses, semesters = uniqueSemesters });
        }









        public async Task<IActionResult> Dashboard()
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

            int totalCreditsEarned = results.Sum(r => r.CourseOffering.Course.Credit);
            int remainingCredits = 153 - totalCreditsEarned;
            double totalGradePoints = results.Sum(r => ConvertGradeToPoint(r.Grade) * r.CourseOffering.Course.Credit);
            double gpa = (totalCreditsEarned > 0) ? totalGradePoints / totalCreditsEarned : 0;

            // ✅ Get current semester
            string currentSemester = _semesterService.GetCurrentSemester(student);
            Console.WriteLine($"🎯 Current Semester of Student {student.StudentID} = {currentSemester}");

            // ✅ Get pending credits for next semester
            int trackedCredits = await _creditTrackingService.GetTrackedCreditsAsync(student);
            Console.WriteLine($"📢 Pending Credits for Next Semester = {trackedCredits}");

            // ✅ ✅ ✅ Fetch Open Registration Sessions for this Student
            var openSessions = await _context.RegistrationSessions
                .Where(rs => rs.AcademicYear == student.AcademicYear && rs.IsOpen && rs.EndDate >= DateTime.UtcNow)
                .ToListAsync();


            // ✅ Pass to ViewData
            ViewData["Student"] = student;
            ViewData["GPA"] = gpa;
            ViewData["TotalCreditsEarned"] = totalCreditsEarned;
            ViewData["RemainingCredits"] = remainingCredits;
            ViewData["CurrentSemester"] = currentSemester;
            ViewData["TrackedCredits"] = trackedCredits;
            ViewData["OpenSessions"] = openSessions;

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

        public async Task<IActionResult> CourseRegistration()
        {
            string studentEmail = User.Identity.Name;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            ViewData["Student"] = student;
            return View("~/Views/Dashboard/Student/CourseRegistration.cshtml");
        }


        public async Task<IActionResult> StudentNotification()
        {
            string studentEmail = User.Identity.Name;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            ViewData["Student"] = student;
            return View("~/Views/Dashboard/Student/StudentNotification.cshtml");
        }

        public async Task<IActionResult> FAQs()
        {
            string studentEmail = User.Identity.Name;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            ViewData["Student"] = student;
            return View("~/Views/Dashboard/Student/FAQs.cshtml");
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

            // ✅ Fetch Registered Courses from Registration Table
            var registeredCourses = await _context.Registrations
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                    .ThenInclude(co => co.Course)
                .Include(r => r.CourseOffering.Coordinator)
                .ToListAsync();

            // ✅ Fetch Pending & Rejected Registrations with Course Details
            var pendingRegistrations = await _context.PendingRegistrations
                .Where(r => r.StudentID == student.StudentID && (r.Status == "Pending" || r.Status == "Rejected"))
                .Include(r => r.RegistrationSessionCourse)
                    .ThenInclude(rsc => rsc.Course) // ✅ Fetch Course details via RegistrationSessionCourse
                .ToListAsync();

            // ✅ Fetch Course Prerequisites from HasPrerequisites
            var coursePrerequisites = await _context.HasPrerequisites
                .GroupBy(p => p.CourseCode)
                .ToDictionaryAsync(g => g.Key, g => g.Select(p => p.PrerequisiteCode).ToList());

            ViewData["Student"] = student;
            ViewData["RegisteredCourses"] = registeredCourses;
            ViewData["PendingRegistrations"] = pendingRegistrations;
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
        public async Task<IActionResult> SubmitCourseRegistration([FromBody] List<CourseSelectionDto> selectedCourses)
        {
            string studentEmail = User.Identity.Name;
            if (string.IsNullOrEmpty(studentEmail))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == studentEmail);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            Console.WriteLine($"📥 Student {student.StudentID} is registering for {selectedCourses.Count} courses.");

            var selectedCourseCodes = selectedCourses.Select(c => c.CourseCode).ToList();

            var validCourses = await _context.RegistrationSessionCourses
                .Where(rsc => selectedCourseCodes.Contains(rsc.CourseCode) &&
                    _context.RegistrationSessions
                        .Where(rs => rs.AcademicYear == student.AcademicYear && rs.IsOpen)
                        .Select(rs => rs.SessionID)
                        .Contains(rsc.SessionID))
                .Include(rsc => rsc.RegistrationSession)
                .ToListAsync();

            if (!validCourses.Any())
            {
                return BadRequest(new { message = "Selected courses are not part of the current session." });
            }

            // ✅ Check for duplicates
            var existingRegistrations = await _context.PendingRegistrations
                .Where(pr => pr.StudentID == student.StudentID && selectedCourseCodes.Contains(pr.CourseCode))
                .Select(pr => new { pr.CourseCode, pr.SessionID })
                .ToListAsync();

            var newCourses = validCourses
                .Where(vc => !existingRegistrations.Any(er => er.CourseCode == vc.CourseCode && er.SessionID == vc.SessionID))
                .ToList();

            if (!newCourses.Any())
            {
                return BadRequest(new { message = "All selected courses have already been submitted for registration." });
            }

            // ✅ Fetch previous results
            var studentResults = await _context.Results
                .Where(r => r.StudentID == student.StudentID)
                .Include(r => r.CourseOffering)
                .ToListAsync();

            // ✅ Check prerequisites
            var prerequisites = await _context.HasPrerequisites
                .Where(p => selectedCourseCodes.Contains(p.CourseCode))
                .ToListAsync();

            List<string> failedPrereqCourses = new List<string>();
            foreach (var prereq in prerequisites)
            {
                var result = studentResults.FirstOrDefault(r => r.CourseOffering.CourseCode == prereq.PrerequisiteCode);
                if (result != null)
                {
                    double average = (result.InCourse + result.EndMarks) / 2.0;
                    if (average < 50)
                    {
                        failedPrereqCourses.Add(prereq.CourseCode);
                    }
                }
            }

            if (failedPrereqCourses.Any())
            {
                return BadRequest(new
                {
                    message = "You have failed prerequisite courses required for registration.",
                    failedCourses = failedPrereqCourses
                });
            }

            // ✅ Create Pending Registrations
            var pendingRegistrations = new List<PendingRegistration>();

            foreach (var course in newCourses)
            {
                string courseCode = course.CourseCode;
                int pastAttempts = studentResults.Count(r => r.CourseOffering.CourseCode == courseCode);
                int attempt = pastAttempts + 1;

                Console.WriteLine($"📌 Course: {courseCode} | PastAttempts: {pastAttempts} | Calculated Attempt: {attempt}");

                CourseOffering courseOffering;

                if (attempt == 1)
                {
                    courseOffering = await _context.CourseOfferings
                        .FirstOrDefaultAsync(co =>
                            co.CourseCode == courseCode &&
                            co.AcademicID == student.AcademicYear &&
                            co.Semester == course.RegistrationSession.Semester);
                    Console.WriteLine($"➡ Attempt = 1 → Using Offering from Same Academic Year {student.AcademicYear}, Semester {course.RegistrationSession.Semester}");
                }
                else
                {
                    courseOffering = await _context.CourseOfferings
                        .Where(co => co.CourseCode == courseCode)
                        .OrderByDescending(co => co.AcademicID)
                        .ThenByDescending(co => co.Semester)
                        .FirstOrDefaultAsync();
                    Console.WriteLine($"➡ Attempt > 1 → Using Latest Offering: OfferingID = {courseOffering?.OfferingID}");
                }

                if (courseOffering == null)
                {
                    Console.WriteLine($"❌ CourseOffering not found for Course: {courseCode}");
                    continue; // Skip this course
                }

                pendingRegistrations.Add(new PendingRegistration
                {
                    SessionID = course.SessionID,
                    CourseCode = courseCode,
                    StudentID = student.StudentID,
                    CourseOfferingID = courseOffering.OfferingID,
                    Status = "Pending",
                    RegistrationDate = DateTime.UtcNow,
                    Attempt = attempt,
                    IsApprovedByAdvisor = "No"
                });

                Console.WriteLine($"✅ Added Pending Registration | Course: {courseCode} | OfferingID: {courseOffering.OfferingID} | Attempt: {attempt}");
            }

            _context.PendingRegistrations.AddRange(pendingRegistrations);
            await _context.SaveChangesAsync();

            Console.WriteLine($"📤 {pendingRegistrations.Count} course(s) submitted for student {student.StudentID}.");

            return Ok(new { message = "Course registration submitted successfully!" });
        }


        // ✅ DTO Class for receiving CourseCode + OfferingID
        public class CourseSelectionDto
        {
            public string CourseCode { get; set; }
            public int CourseOfferingID { get; set; }
        }



        private async Task<List<AvailableCourse>> GetAvailableCoursesList(int studentID)
        {
            // Fetch student's past results with course codes and average marks
            var pastResults = await _context.Results
                .Where(r => r.StudentID == studentID)
                .Include(r => r.CourseOffering)
                .Select(r => new
                {
                    CourseCode = r.CourseOffering.CourseCode,
                    Average = (r.InCourse + r.EndMarks) / 2.0
                })
                .ToListAsync();

            // Courses that student has passed already
            var passedCourses = pastResults
                .Where(r => r.Average >= 50)
                .Select(r => r.CourseCode)
                .Distinct()
                .ToList();

            // Count the number of attempts for each course
            var pastAttempts = await _context.Results
                .Where(r => r.StudentID == studentID)
                .GroupBy(r => r.CourseOffering.CourseCode)
                .Select(g => new { CourseCode = g.Key, AttemptCount = g.Count() })
                .ToDictionaryAsync(x => x.CourseCode, x => x.AttemptCount);

            // Courses already in pending registration
            var alreadyRegisteredCourses = await _context.PendingRegistrations
                .Where(pr => pr.StudentID == studentID)
                .Select(pr => pr.CourseCode)
                .ToListAsync();

            // Fetch open registration sessions
            var openSessions = await _context.RegistrationSessions
                .Where(rs => rs.IsOpen && rs.EndDate >= DateTime.UtcNow)
                .Include(rs => rs.RegistrationSessionCourses)
                .ThenInclude(rsc => rsc.Course)
                .ToListAsync();

            // Fetch available courses with filtering
            var availableCourses = openSessions
                .SelectMany(rs => rs.RegistrationSessionCourses
                    .Where(rsc =>
                        !alreadyRegisteredCourses.Contains(rsc.Course.CourseCode) &&  // Not in pending
                        (!passedCourses.Contains(rsc.Course.CourseCode) ||  // If failed, allow reattempt
                         pastResults.Any(r => r.CourseCode == rsc.Course.CourseCode && r.Average < 50))
                    )
                    .Select(rsc => new AvailableCourse
                    {
                        CourseCode = rsc.Course.CourseCode,
                        CourseName = rsc.Course.CourseName,
                        Credit = rsc.Course.Credit,
                        Semester = rs.Semester,
                        ClosingDate = rs.EndDate.ToString("yyyy-MM-dd"),
                        Coordinator = "Not Assigned",
                        Attempt = pastAttempts.ContainsKey(rsc.Course.CourseCode)
                            ? pastAttempts[rsc.Course.CourseCode] + 1  // Increment attempt count
                            : 1 // First attempt
                    }))
                .ToList();

            return availableCourses;
        }

        // Define a class to store available course data
        private class AvailableCourse
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public int Credit { get; set; }
            public string Semester { get; set; }
            public string ClosingDate { get; set; }
            public string Coordinator { get; set; }
            public int Attempt { get; set; }
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
            // ✅ Fetch Admin Name from Session
            ViewBag.AdminName = User.Identity.Name;

            // ✅ Fetch Departments (Exclude Administration)
            ViewBag.Departments = await _context.Departments
                .Where(d => d.DepartmentID != 6) // Excluding Administration
                .ToListAsync();

            // ✅ Fetch Distinct Academic Years from Student Table
            ViewBag.AcademicYears = await _context.Students
                .Select(s => s.AcademicYear)
                .Distinct()
                .ToListAsync();

            return View("~/Views/Dashboard/Admin/CourseOffering.cshtml");
        }

        /// <summary>
        /// Fetches Courses Based on Selected Semester and Department
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFilteredCourses(int semester, int? departmentId)
        {
            Console.WriteLine($"🔹 Fetching courses for Semester: {semester}, Department: {departmentId}");

            var courses = await _context.Courses
                .Where(c => c.Semester == semester.ToString() &&
                            (departmentId == null || c.DepID == departmentId)) // Ensure department filtering
                .Include(c => c.Department)
                .Select(c => new
                {
                    CourseCode = c.CourseCode ?? "N/A",
                    CourseName = c.CourseName ?? "N/A",
                    Credit = c.Credit != null ? c.Credit : 0,
                    DepartmentName = c.Department != null ? c.Department.DepartmentName : "Unknown"
                })
                .ToListAsync();

            Console.WriteLine($"✅ Courses Fetched: {courses.Count}");
            foreach (var course in courses)
            {
                Console.WriteLine($"➡ {course.CourseCode} | {course.CourseName} | {course.Credit} | {course.DepartmentName}");
            }

            return Json(courses);
        }



        /*

        [HttpPost]
        public async Task<IActionResult> CreateRegistrationSession(
            [FromForm] string[] academicYears,
            [FromForm] int semester,
            [FromForm] int? departmentId,
            [FromForm] DateTime closingDate,
            [FromForm] List<string> selectedCourses)
        {
            if (academicYears.Length == 0 || semester == 0 ||
    closingDate == default || closingDate < DateTime.Today ||
    selectedCourses.Count == 0)
            {
                return Json(new { success = false, message = "Please fill all fields and ensure the closing date is not in the past." });
            }

            bool isGeneralProgram = semester <= 8;
            departmentId = isGeneralProgram ? null : departmentId; // Set department to NULL for General Program

            foreach (var academicYear in academicYears)
            {
                var existingSession = await _context.RegistrationSessions
                    .FirstOrDefaultAsync(rs =>
                        rs.AcademicYear == academicYear &&
                        rs.Semester == semester.ToString() &&
                        rs.DepartmentID == departmentId); // ✅ Now also checks for DepartmentID

                if (existingSession != null)
                {
                    return Json(new { success = false, message = $"Registration session already exists for {academicYear}, Semester {semester}, Department: {departmentId}." });
                }


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
                await _context.SaveChangesAsync();

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
        */

        [HttpPost]
        public async Task<IActionResult> CreateRegistrationSession(
    [FromForm] string[] academicYears,
    [FromForm] int semester,
    [FromForm] int? departmentId,
    [FromForm] DateTime closingDate,
    [FromForm] List<string> selectedCourses)
        {
            if (academicYears.Length == 0 || semester == 0 ||
                closingDate == default || closingDate < DateTime.Today ||
                selectedCourses.Count == 0)
            {
                return Json(new { success = false, message = "Please fill all fields and ensure the closing date is not in the past." });
            }

            bool isGeneralProgram = semester <= 3;
            departmentId = isGeneralProgram ? null : departmentId; // Set department to NULL for General Program

            foreach (var academicYear in academicYears)
            {
                // 🚀 ✅ Instead of preventing duplicates, allow multiple sessions.
                // This block is now removed:
                // var existingSession = await _context.RegistrationSessions
                //     .FirstOrDefaultAsync(rs =>
                //         rs.AcademicYear == academicYear &&
                //         rs.Semester == semester.ToString() &&
                //         rs.DepartmentID == departmentId);

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
                await _context.SaveChangesAsync();

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


        [HttpGet]
        public async Task<IActionResult> GetRegistrationSessions()
        {
            var sessions = await _context.RegistrationSessions
                .Include(rs => rs.Department)
                .Include(rs => rs.RegistrationSessionCourses)
                .ThenInclude(rsc => rsc.Course)
                .Select(rs => new
                {
                    rs.SessionID,
                    rs.AcademicYear,
                    rs.Semester,
                    DepartmentName = rs.Department != null ? rs.Department.DepartmentName : "General Program",
                    rs.StartDate,
                    rs.EndDate,
                    Status = rs.IsOpen ? "Open" : "Closed",
                    Courses = rs.RegistrationSessionCourses.Select(rsc => rsc.Course.CourseName).ToList() // Fetch Course Names
                })
                .ToListAsync();

            return Json(sessions);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRegistrationSession(int sessionID, DateTime newEndDate, string newStatus)
        {
            var session = await _context.RegistrationSessions.FindAsync(sessionID);
            if (session == null) return NotFound(new { message = "Session not found." });

            session.EndDate = newEndDate;
            session.IsOpen = newStatus == "Open";

            await _context.SaveChangesAsync();
            return Json(new { message = "Session updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRegistrationSession(int sessionID)
        {
            var session = await _context.RegistrationSessions.FindAsync(sessionID);
            if (session == null) return NotFound(new { message = "Session not found." });

            _context.RegistrationSessions.Remove(session);
            await _context.SaveChangesAsync();
            return Json(new { message = "Session deleted successfully!" });
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

        

        /// <summary>
        /// Loads Old Registration History Page
        /// </summary>
        public async Task<IActionResult> OldRegistrationHistory()
        {

            return View("~/Views/Dashboard/Admin/OldRegistrationHistory.cshtml");
        }

        /// <summary>
        /// Loads Pending Registrations Page
        /// </summary>
        public async Task<IActionResult> PendingRegistrations()
        {
            return View("~/Views/Dashboard/Admin/PendingRegistrations.cshtml");
        }

        /// <summary>
        /// Loads Registration Session Page
        /// </summary>
        public async Task<IActionResult> RegistrationSession()
        {
            return View("~/Views/Dashboard/Admin/RegistrationSession.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingRegistrations()
        {
            Console.WriteLine("🔹 Fetching Pending Registrations...");

            var pendingRegistrations = await _context.PendingRegistrations
                .Include(pr => pr.Student)
                .Include(pr => pr.RegistrationSessionCourse)
                    .ThenInclude(rsc => rsc.Course)
                .Include(pr => pr.RegistrationSessionCourse.RegistrationSession)
                .Select(pr => new
                {
                    PendingID = pr.PendingID,
                    StudentID = pr.Student.StudentID,
                    StudentName = pr.Student.FullName,
                    CourseCode = pr.CourseCode,
                    CourseName = pr.RegistrationSessionCourse.Course.CourseName,
                    Semester = pr.RegistrationSessionCourse.RegistrationSession.Semester,
                    ClosingDate = pr.RegistrationSessionCourse.RegistrationSession.EndDate.ToString("yyyy-MM-dd"),
                    Status = pr.Status,
                    RegistrationDate = pr.RegistrationDate.ToString("yyyy-MM-dd"),
                    Attempt = pr.Attempt,
                    IsApprovedByAdvisor = pr.IsApprovedByAdvisor
                })
                .ToListAsync();

            Console.WriteLine($"✅ API Data Fetched: {pendingRegistrations.Count} pending registrations.");

            // Print each registration for debugging
            foreach (var reg in pendingRegistrations)
            {
                Console.WriteLine($"➡ {reg.PendingID} | {reg.StudentID} | {reg.CourseCode} | {reg.Semester} | Attempt: {reg.Attempt} | Approved: {reg.IsApprovedByAdvisor}");
            }

            return Json(pendingRegistrations);
        }



        [HttpPost]
        public async Task<IActionResult> UpdatePendingStatus([FromBody] UpdatePendingStatusRequest request)
        {
            Console.WriteLine($"? Processing PendingID: {request.PendingID}, Status: {request.Status}");

            var record = await _context.PendingRegistrations
                .Include(p => p.Student)
                .Include(p => p.RegistrationSessionCourse)
                .ThenInclude(rsc => rsc.Course)
                .Include(p => p.RegistrationSessionCourse.RegistrationSession)
                .FirstOrDefaultAsync(p => p.PendingID == request.PendingID);

            if (record == null)
            {
                return NotFound(new { message = "Pending registration not found." });
            }

            // ✅ Check if the request is for approval
            if (request.Status == "Approved")
            {
                Console.WriteLine($"? Approving Registration for StudentID: {record.StudentID}, Course: {record.CourseCode}");

                // ✅ Check if Course Offering Exists for the Student’s Academic Year
                var courseOffering = await _context.CourseOfferings
                    .FirstOrDefaultAsync(co => co.CourseCode == record.CourseCode
                                               && co.AcademicID == record.RegistrationSessionCourse.RegistrationSession.AcademicYear);

                // ✅ If the course offering for the academic year doesn't exist, create it
                if (courseOffering == null)
                {
                    Console.WriteLine($"❌ Course Offering Not Found for CourseCode: {record.CourseCode}, AcademicYear: {record.RegistrationSessionCourse.RegistrationSession.AcademicYear}. Creating new offering...");

                    courseOffering = new CourseOffering
                    {
                        CourseCode = record.CourseCode,
                        AcademicID = record.RegistrationSessionCourse.RegistrationSession.AcademicYear, // ✅ New academic year
                        StartDate = record.RegistrationDate,
                        EndDate = record.RegistrationSessionCourse.RegistrationSession.EndDate,
                        Semester = record.RegistrationSessionCourse.RegistrationSession.Semester,
                        StaffID = null  // ✅ Assign default or NULL staff
                    };

                    _context.CourseOfferings.Add(courseOffering);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"✅ New Course Offering Created for {record.CourseCode}, AcademicYear: {record.RegistrationSessionCourse.RegistrationSession.AcademicYear}");
                }
                else
                {
                    Console.WriteLine($"✅ Course Offering Found: {courseOffering.OfferingID} for AcademicYear: {courseOffering.AcademicID}");
                }

                // ✅ Fetch the Latest Offering ID for the Course (Regardless of Academic Year)
                var latestOffering = await _context.CourseOfferings
                    .Where(co => co.CourseCode == record.CourseCode)
                    .OrderByDescending(co => co.OfferingID) // ✅ Get the latest created offering
                    .FirstOrDefaultAsync();

                if (latestOffering != null)
                {
                    Console.WriteLine($"✅ Using Last Created OfferingID: {latestOffering.OfferingID} for StudentID: {record.StudentID}");
                    courseOffering = latestOffering; // ✅ Assign the latest offering for registration
                }

                // ✅ Use the attempt count directly from PendingRegistrations table
                int currentAttempt = record.Attempt;

                // ✅ Add to Registration Table
                var newRegistration = new Registration
                {
                    OfferingID = courseOffering.OfferingID,  // ✅ Always link to the last available offering
                    StudentID = record.StudentID,
                    Semester = record.RegistrationSessionCourse.RegistrationSession.Semester,
                    Attempt = currentAttempt, // ✅ Use Attempt from PendingRegistrations table
                    RegistrationDate = DateTime.UtcNow,
                    ApprovalDate = DateTime.UtcNow,
                    TimeStamp = DateTime.UtcNow
                };

                _context.Registrations.Add(newRegistration);
                Console.WriteLine($"✅ Registration Created for StudentID: {record.StudentID}, Course: {record.CourseCode}, Attempt: {currentAttempt}, OfferingID: {courseOffering.OfferingID}");
            }

            // ✅ Update Pending Registration Status
            record.Status = request.Status;
            record.ApprovalDate = request.Status == "Approved" ? DateTime.UtcNow : (DateTime?)null;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Pending registration {request.Status} successfully!" });
        }




        // ✅ C# Model for JSON Request Validation
        public class UpdatePendingStatusRequest
        {
            public int PendingID { get; set; }
            public string Status { get; set; }
        }


        [HttpGet]
        public IActionResult ExportSession(int sessionID)
        {
            try
            {
                var session = _context.RegistrationSessions
                    .Include(rs => rs.Department)
                    .Include(rs => rs.RegistrationSessionCourses)
                        .ThenInclude(rsc => rsc.Course)
                    .FirstOrDefault(rs => rs.SessionID == sessionID);

                if (session == null)
                    return NotFound("Session not found.");

                var courseCodes = session.RegistrationSessionCourses
                    .Select(rsc => rsc.CourseCode)
                    .Distinct()
                    .ToList();

                var approvedRegistrations = _context.PendingRegistrations
                    .Where(pr => pr.SessionID == sessionID && pr.Status == "Approved")
                    .Include(pr => pr.Student)
                    .ToList();

                var students = approvedRegistrations
                    .Select(pr => pr.Student)
                    .Distinct()
                    .ToList();

                using (MemoryStream stream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate());
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    document.Add(new Paragraph("\uD83D\uDCC4 Registration Session Details\n\n"));
                    document.Add(new Paragraph($"Session ID     : {session.SessionID}"));
                    document.Add(new Paragraph($"Academic Year  : {session.AcademicYear}"));
                    document.Add(new Paragraph($"Semester       : {session.Semester}"));
                    document.Add(new Paragraph($"Department     : {(session.Department != null ? session.Department.DepartmentName : "General Program")}"));
                    document.Add(new Paragraph($"Start Date     : {session.StartDate:yyyy-MM-dd}"));
                    document.Add(new Paragraph($"End Date       : {session.EndDate:yyyy-MM-dd}"));
                    document.Add(new Paragraph($"Status         : {(session.IsOpen ? "Open" : "Closed")}"));

                    document.Add(new Paragraph("\nOffered Courses:"));
                    foreach (var course in session.RegistrationSessionCourses)
                    {
                        document.Add(new Paragraph($"  - {course.Course.CourseCode} - {course.Course.CourseName}"));
                    }

                    document.Add(new Paragraph("\n\nApproved Registrations Table\n\n"));

                    PdfPTable table = new PdfPTable(courseCodes.Count + 2);
                    table.WidthPercentage = 100;

                    PdfPCell nameHeader = new PdfPCell(new Phrase("Student Name"));
                    nameHeader.Colspan = 1;
                    nameHeader.MinimumHeight = 25f;
                    nameHeader.FixedHeight = 30f;
                    nameHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(nameHeader);

                    table.AddCell("Student ID");

                    foreach (var code in courseCodes)
                        table.AddCell(code);

                    foreach (var student in students)
                    {
                        PdfPCell nameCell = new PdfPCell(new Phrase(student.FullName));
                        nameCell.MinimumHeight = 25f;
                        nameCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.AddCell(nameCell);

                        table.AddCell(student.StudentID.ToString());
                        foreach (var code in courseCodes)
                        {
                            bool registered = approvedRegistrations.Any(r => r.StudentID == student.StudentID && r.CourseCode == code);
                            table.AddCell(registered ? "Yes" : "No");
                        }
                    }

                    document.Add(table);
                    document.Close();

                    byte[] pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Session_{sessionID}.pdf");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR EXPORTING PDF: {ex.Message}");
                return BadRequest("Error generating PDF. Please try again.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAdvisorStudentList()
        {
            var data = await _context.Students
                .Where(s => s.StaffID != null)
                .Include(s => s.Advisor)
                .Select(s => new {
                    regNo = s.StudentID.ToString(),
                    studentName = s.FullName,
                    advisorName = s.Advisor.FullName,
                    department = s.Department.DepartmentName,
                    year = s.AcademicYear
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public IActionResult GetStudentDepartments()
        {
            var departments = _context.Students
                .Include(s => s.Department)
                .Where(s => s.Department != null)
                .Select(s => s.Department.DepartmentName)
                .Distinct()
                .ToList();

            return Json(departments);
        }

        [HttpGet]
        public IActionResult GetAcademicYears()
        {
            var years = _context.Students
                .Select(s => s.AcademicYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            return Json(years);
        }




    }
}

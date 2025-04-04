using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using FOE_CourseRegistrationSystem.Services;

public class CreditTrackingService
{
    private readonly ApplicationDbContext _context;
    private readonly SemesterService _semesterService;

    public CreditTrackingService(ApplicationDbContext context, SemesterService semesterService)
    {
        _context = context;
        _semesterService = semesterService;
    }

    public async Task<int> GetTrackedCreditsAsync(Student student)
    {
        var currentSemesterStr = _semesterService.GetCurrentSemester(student);
        if (!int.TryParse(currentSemesterStr, out int currentSemester))
        {
            return 0;
        }

        int nextSemester = currentSemester + 1;
        Console.WriteLine($"? Next Semester Calculated = {nextSemester}");

        var nextSchedule = await _context.AcademicSchedules
            .FirstOrDefaultAsync(s => s.AcademicYear == student.AcademicYear && s.Semester == nextSemester);

        if (nextSchedule == null)
        {
            Console.WriteLine($"❌ No Academic Schedule Found for AcademicYear = {student.AcademicYear}, Semester = {nextSemester}");
            return 0;
        }

        Console.WriteLine($"✅ Next Semester Schedule Found:");
        Console.WriteLine($"   Semester   : {nextSchedule.Semester}");
        Console.WriteLine($"   Start Date : {nextSchedule.SemesterStartDate}");
        Console.WriteLine($"   End Date   : {nextSchedule.SemesterEndDate}");

        // ✅ Get all next semester's CourseOfferings
        var nextSemesterOfferings = await _context.CourseOfferings
            .Where(o => o.StartDate == nextSchedule.SemesterStartDate && o.EndDate == nextSchedule.SemesterEndDate)
            .Select(o => o.OfferingID)
            .ToListAsync();

        // ✅ Now track credits from pending registrations matching these offerings
        var totalCredits = await _context.PendingRegistrations
            .Where(p => p.StudentID == student.StudentID)
            .Join(_context.RegistrationSessionCourses,
                  pending => new { pending.SessionID, pending.CourseCode },
                  rsc => new { rsc.SessionID, rsc.CourseCode },
                  (pending, rsc) => new { pending, rsc })
            .Join(_context.CourseOfferings,
                  join => join.rsc.CourseCode,
                  offer => offer.CourseCode,
                  (join, offer) => new { join.pending, join.rsc, offer })
            .Where(x => nextSemesterOfferings.Contains(x.offer.OfferingID)) // ✅ Only next semester courses
            .SumAsync(x => (int?)x.rsc.Course.Credit) ?? 0;

        Console.WriteLine($"✅ Pending Credits for Next Semester = {totalCredits}");

        return totalCredits;
    }





}

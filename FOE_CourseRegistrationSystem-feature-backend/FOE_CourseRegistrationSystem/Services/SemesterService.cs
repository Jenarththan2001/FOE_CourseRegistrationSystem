using FOE_CourseRegistrationSystem.Data;
using FOE_CourseRegistrationSystem.Models;

namespace FOE_CourseRegistrationSystem.Services
{
    public class SemesterService
    {
        private readonly ApplicationDbContext _context;

        public SemesterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetCurrentSemester(Student student)
        {
            var today = DateTime.Today;
            var schedule = _context.AcademicSchedules
                .Where(s => s.AcademicYear == student.AcademicYear &&
                            today >= s.SemesterStartDate &&
                            today <= s.SemesterEndDate)
                .FirstOrDefault();

            return schedule != null ? schedule.Semester.ToString() : "N/A";
        }
    }
}

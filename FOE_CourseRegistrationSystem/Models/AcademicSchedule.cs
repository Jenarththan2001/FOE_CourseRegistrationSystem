using System;
using System.ComponentModel.DataAnnotations;

namespace FOE_CourseRegistrationSystem.Models
{
    public class AcademicSchedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [Required]
        [StringLength(9)]
        public string AcademicYear { get; set; }   // E.g., "2023/2024"

        [Required]
        public int Semester { get; set; }          // 1 to 8

        [Required]
        public DateTime SemesterStartDate { get; set; }

        [Required]
        public DateTime SemesterEndDate { get; set; }
    }
}

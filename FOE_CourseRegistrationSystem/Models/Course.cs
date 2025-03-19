using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Course
    {
        [Key]
        public string CourseCode { get; set; }  // Unique Course Code (e.g., EC5080)

        [Required, StringLength(100)]
        public string CourseName { get; set; }  // Name of the course

        public int Credit { get; set; }  // Credit hours

        [Required]
        public string Semester { get; set; }  // Semester when offered

        [Required]
        public string CoreOrElective { get; set; }  // Core or Elective

        [ForeignKey("Department")]
        public int DepID { get; set; }  // Department offering the course
        public Department Department { get; set; }
    }
}

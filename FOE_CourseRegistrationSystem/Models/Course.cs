using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Course
    {
        [Key]
        public string CourseCode { get; set; }

        [Required, StringLength(100)]
        public string CourseName { get; set; }

        public int Credit { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public string CoreOrElective { get; set; }

        [ForeignKey("Department")]
        public int DepID { get; set; }
        public Department Department { get; set; }
    }
}

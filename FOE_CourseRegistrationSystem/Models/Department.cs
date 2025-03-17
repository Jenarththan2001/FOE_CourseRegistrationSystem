using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required, StringLength(100)]
        public string DepartmentName { get; set; }

        [ForeignKey("HOD")]
        public int? StaffID { get; set; }
        public Staff HOD { get; set; }

        // List of students in this department
        public ICollection<Student> Students { get; set; }

        // List of staff in this department
        public ICollection<Staff> Staffs { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}

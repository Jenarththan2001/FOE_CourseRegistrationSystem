using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }  // Primary Key

        [Required, StringLength(100)]
        public string DepartmentName { get; set; }  // Name of the department

        [ForeignKey("HOD")]
        public int? StaffID { get; set; }  // The Head of the Department (HOD) - Foreign Key
        public Staff HOD { get; set; }

        public ICollection<Student> Students { get; set; }  // List of students in the department
        public ICollection<Staff> Staffs { get; set; }  // List of staff members
        public ICollection<Course> Courses { get; set; } = new List<Course>();  // List of courses offered
    }
}

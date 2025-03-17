using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, StringLength(12)]
        public string NIC { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Nationality { get; set; }
        public string Photo { get; set; }
        public string TempAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Gender { get; set; }

        [Phone]
        public string PhoneNo { get; set; }

        public int AcademicYear { get; set; }

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        [ForeignKey("Advisor")]
        public int? StaffID { get; set; }
        public Staff Advisor { get; set; }
    }
}

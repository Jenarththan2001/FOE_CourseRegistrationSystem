using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }  // Primary Key

        [Required, EmailAddress]
        public string Email { get; set; }  // Unique student email

        [Required]
        public string Password { get; set; }  // Stored in hashed format

        [Required, StringLength(12)]
        public string NIC { get; set; }  // National ID of the student

        [Required]
        public string FullName { get; set; }  // Full Name

        public string Nationality { get; set; }  // Nationality
        public string Photo { get; set; } = "default.jpg";  // Profile Picture
        public string TempAddress { get; set; }  // Temporary Address
        public string PermanentAddress { get; set; }  // Permanent Address
        public string Gender { get; set; }  // Gender (Male/Female)

        [Phone]
        public string PhoneNo { get; set; }  // Contact Number


        [Required, StringLength(9)]  // ✅ Change AcademicYear to string
        public string AcademicYear { get; set; }

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }  // Foreign Key to Department
        public Department Department { get; set; }

        [ForeignKey("Advisor")]
        public int? StaffID { get; set; }  // Advisor (Lecturer) assigned to student
        public Staff Advisor { get; set; }
    }

}

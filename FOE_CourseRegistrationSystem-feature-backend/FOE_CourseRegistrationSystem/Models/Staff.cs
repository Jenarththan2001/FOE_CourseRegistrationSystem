using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public enum StaffRole
    {
        Advisor,
        AR,
        Coordinator
    }

    public class Staff
    {
        [Key]
        public int StaffID { get; set; }  // Primary Key

        [Required, EmailAddress]
        public string Email { get; set; }  // Staff email

        [Required]
        public string Password { get; set; }  // Stored in hashed format

        [Required, StringLength(12)]
        public string NIC { get; set; }  // National ID

        [Required]
        public string FullName { get; set; }  // Staff name

        [Phone]
        public string PhoneNo { get; set; }  // Contact Number

        [ForeignKey("Department")]
        public int DepID { get; set; }  // Department they belong to
        public Department Department { get; set; }

        [Required]
        public StaffRole Role { get; set; }  // Enum: Advisor, AR, Coordinator

        public ICollection<Student> AdvisedStudents { get; set; }  // Students they advise
    }
}

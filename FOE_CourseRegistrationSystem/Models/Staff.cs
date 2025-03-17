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
        public int StaffID { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, StringLength(12)]
        public string NIC { get; set; }

        [Required]
        public string FullName { get; set; }

        [Phone]
        public string PhoneNo { get; set; }

        [ForeignKey("Department")]
        public int DepID { get; set; }
        public Department Department { get; set; }

        [Required]
        public StaffRole Role { get; set; }

        // List of students advised by the staff
        public ICollection<Student> AdvisedStudents { get; set; }
    }
}

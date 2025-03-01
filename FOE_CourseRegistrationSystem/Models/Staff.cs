using System.ComponentModel.DataAnnotations;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // ✅ Add this line

        [Required]
        public string NIC { get; set; }

        public string? FullName { get; set; }
        public string? PhoneNo { get; set; }
        public string? Role { get; set; }
    }
}

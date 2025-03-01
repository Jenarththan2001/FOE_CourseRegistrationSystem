using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    [Table("STUDENT")]  // ✅ Map EF model to existing SQL table
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("Password")]  // ✅ Match SQL column name exactly
        public string Password { get; set; }

        [Required]
        public string NIC { get; set; }

        public string? FullName { get; set; }
        public string? Nationality { get; set; }
        public byte[]? Photo { get; set; }
        public string? TempAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNo { get; set; }
        public int AcademicYear { get; set; }
    }
}

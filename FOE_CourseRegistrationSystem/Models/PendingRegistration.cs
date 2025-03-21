using FOE_CourseRegistrationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class PendingRegistration
{
    [Key]
    public int PendingID { get; set; } // ✅ Primary Key

    public int SessionID { get; set; }
    public string CourseCode { get; set; }

    [ForeignKey(nameof(SessionID) + "," + nameof(CourseCode))] // ✅ Composite Foreign Key
    public RegistrationSessionCourse RegistrationSessionCourse { get; set; }

    [ForeignKey("Student")]
    public int StudentID { get; set; }
    public Student Student { get; set; }

    public string Status { get; set; } = "Pending"; // ✅ Default: Pending, Approved, or Rejected

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovalDate { get; set; } // ✅ Nullable if not yet approved

    public string? AdminRemarks { get; set; } // ✅ Ensure AdminRemarks can be NULL
}

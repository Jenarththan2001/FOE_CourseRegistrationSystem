using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
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

        // ✅ ✅ ✅ Make CourseOfferingID nullable to avoid FK conflict during migration
        [ForeignKey("CourseOffering")]
        public int? CourseOfferingID { get; set; }
        public CourseOffering CourseOffering { get; set; }

        public string Status { get; set; } = "Pending"; // ✅ Default: Pending, Approved, or Rejected

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; } // ✅ Nullable if not yet approved

        public int Attempt { get; set; } = 1; // ✅ Stores the attempt number (1 = first attempt, 2 = second, etc.)

        public string IsApprovedByAdvisor { get; set; } = "No"; // ✅ Stores "Yes" or "No" for advisor approval
    }
}

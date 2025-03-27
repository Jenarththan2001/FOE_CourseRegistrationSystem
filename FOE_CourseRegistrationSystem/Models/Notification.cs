using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public enum NotificationType
    {
        RegistrationSessionOpened,      // AR opens registration
        CourseRegistrationSubmitted,    // Student submits registration
        AdvisorApproval,                // Advisor approves/rejects
        ARApproval                      // AR approves
    }

    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        // Message Fields
        [Required]
        [MaxLength(1000)]
        public string ReceiverMessage { get; set; }  // Primary recipient message

        [MaxLength(1000)]
        public string SenderMessage { get; set; }    // Sender's copy (optional)

        [MaxLength(500)]
        public string ThirdPartyMessage { get; set; } // For additional recipients (optional)

        // Core Properties
        [Required]
        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Recipient Configuration
        [Required]
        public int ReceiverID { get; set; }          // Primary recipient ID

        [Required]
        [StringLength(10)]
        public string ReceiverType { get; set; }      // "Student" or "Staff"

        // Sender Information (disambiguated)
        public int? SenderStudentID { get; set; }    // Only set if sender is a student
        public int? SenderStaffID { get; set; }       // Only set if sender is staff

        [StringLength(10)]
        public string SenderType { get; set; }        // "Student" or "Staff"

        // Third Party Recipient (disambiguated)
        public int? ThirdPartyStudentID { get; set; } // Only set if third party is a student
        public int? ThirdPartyStaffID { get; set; }   // Only set if third party is staff

        [StringLength(10)]
        public string ThirdPartyType { get; set; }    // "Student" or "Staff"

        // Special Cases
        public bool? IsForAllStudents { get; set; }   // Broadcast flag (nullable)

        [StringLength(9)]
        public string AcademicYear { get; set; }      // e.g. "2020/2021" (nullable)

        // Related Entities
        public int? RelatedStudentID { get; set; }
        public int? RelatedStaffID { get; set; }
        public int? RelatedSessionID { get; set; }
        public int? RelatedPendingRegistrationID { get; set; }

        // Navigation Properties
        [ForeignKey("RelatedStudentID")]
        public Student RelatedStudent { get; set; }

        [ForeignKey("RelatedStaffID")]
        public Staff RelatedStaff { get; set; }

        [ForeignKey("RelatedSessionID")]
        public RegistrationSession RelatedSession { get; set; }

        [ForeignKey("RelatedPendingRegistrationID")]
        public PendingRegistration RelatedPendingRegistration { get; set; }

        // Disambiguated Sender Relationships
        [ForeignKey("SenderStudentID")]
        public Student SenderStudent { get; set; }

        [ForeignKey("SenderStaffID")]
        public Staff SenderStaff { get; set; }

        // Disambiguated Third Party Relationships
        [ForeignKey("ThirdPartyStudentID")]
        public Student ThirdPartyStudent { get; set; }

        [ForeignKey("ThirdPartyStaffID")]
        public Staff ThirdPartyStaff { get; set; }
    }
}
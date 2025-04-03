using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public enum NotificationType
    {
        RegistrationSessionOpened,
        CourseRegistrationSubmitted,
        AdvisorApproval,
        ARApproval
    }

    public enum UserType
    {
        Student,
        Staff
    }

    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required, MaxLength(1000)]
        public string ReceiverMessage { get; set; }

        [MaxLength(1000)]
        public string SenderMessage { get; set; }

        [MaxLength(500)]
        public string ThirdPartyMessage { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int ReceiverID { get; set; }

        [Required]
        public UserType ReceiverType { get; set; }

        public int? SenderStudentID { get; set; }
        public int? SenderStaffID { get; set; }
        public UserType? SenderType { get; set; }

        public int? ThirdPartyStudentID { get; set; }
        public int? ThirdPartyStaffID { get; set; }
        public UserType? ThirdPartyType { get; set; }

        public bool? IsForAllStudents { get; set; }

        [StringLength(9)]
        public string AcademicYear { get; set; }

        public int? RelatedStudentID { get; set; }
        public int? RelatedStaffID { get; set; }
        public int? RelatedSessionID { get; set; }

        [StringLength(20)]
        public string RelatedCourseCode { get; set; }

        [StringLength(20)]
        public string RelatedStatus { get; set; }

        public int? RelatedPendingRegistrationID { get; set; }

        [ForeignKey("RelatedStudentID")]
        public Student RelatedStudent { get; set; }

        [ForeignKey("RelatedStaffID")]
        public Staff RelatedStaff { get; set; }

        [ForeignKey("RelatedSessionID")]
        public RegistrationSession RelatedSession { get; set; }

        [ForeignKey("RelatedPendingRegistrationID")]
        public PendingRegistration RelatedPendingRegistration { get; set; }

        [ForeignKey("SenderStudentID")]
        public Student SenderStudent { get; set; }

        [ForeignKey("SenderStaffID")]
        public Staff SenderStaff { get; set; }

        [ForeignKey("ThirdPartyStudentID")]
        public Student ThirdPartyStudent { get; set; }

        [ForeignKey("ThirdPartyStaffID")]
        public Staff ThirdPartyStaff { get; set; }
    }
}

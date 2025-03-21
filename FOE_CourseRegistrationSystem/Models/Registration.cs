using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Registration
    {
        [Key]
        public int RegistrationID { get; set; }  // Unique ID for the registration

        [ForeignKey("CourseOffering")]
        public int OfferingID { get; set; }  // The specific offering of a course
        public CourseOffering CourseOffering { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }  // The student registering
        public Student Student { get; set; }

        public string Semester { get; set; }  // The semester in which registration happened
        public int Attempt { get; set; }  // Number of times student is attempting this course
        public DateTime RegistrationDate { get; set; }  // Date of registration
        public DateTime? ApprovalDate { get; set; }  // Date of approval (nullable)
        public DateTime TimeStamp { get; set; }  // Timestamp for record tracking
    }

}

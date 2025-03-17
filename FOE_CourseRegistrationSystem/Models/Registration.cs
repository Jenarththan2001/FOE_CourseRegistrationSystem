using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Registration
    {
        [Key]
        public int RegistrationID { get; set; }

        [ForeignKey("CourseOffering")]
        public int OfferingID { get; set; }
        public CourseOffering CourseOffering { get; set; }

        public string Semester { get; set; }
        public int Attempt { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

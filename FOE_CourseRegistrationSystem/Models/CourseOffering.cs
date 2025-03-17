using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class CourseOffering
    {
        [Key]
        public int OfferingID { get; set; }

        [ForeignKey("Course")]
        public string CourseCode { get; set; }
        public Course Course { get; set; }

        public string AcademicID { get; set; }
        public string Semester { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey("Coordinator")]
        public int StaffID { get; set; }
        public Staff Coordinator { get; set; }
    }
}

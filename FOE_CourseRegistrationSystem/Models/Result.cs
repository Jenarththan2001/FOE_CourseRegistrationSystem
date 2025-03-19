using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Result
    {
        [Key, Column(Order = 0)]
        [ForeignKey("CourseOffering")]
        public int OfferingID { get; set; }
        public CourseOffering CourseOffering { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public double InCourse { get; set; }  // Internal assessment marks
        public double EndMarks { get; set; }  // Final exam marks
        public string Grade { get; set; }  // Letter grade (A, B, C, etc.)
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class HasPrerequisite
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Course")]
        public string CourseCode { get; set; }  // Course that has a prerequisite
        public Course Course { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Prerequisite")]
        public string PrerequisiteCode { get; set; }  // The prerequisite course
        public Course Prerequisite { get; set; }
    }

}

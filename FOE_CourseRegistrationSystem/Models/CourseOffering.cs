using FOE_CourseRegistrationSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CourseOffering
{
    [Key]
    public int OfferingID { get; set; }  // Unique Offering ID

    [ForeignKey("Course")]
    public string CourseCode { get; set; }  // Course being offered
    public Course Course { get; set; }

    public string AcademicID { get; set; }  // Academic year and semester
    public string Semester { get; set; }  // Semester
    public DateTime StartDate { get; set; }  // Course start date
    public DateTime EndDate { get; set; }  // Course end date

    [ForeignKey("Coordinator")]
    public int? StaffID { get; set; }  // ✅ Ensure this is nullable
    public Staff Coordinator { get; set; }
}

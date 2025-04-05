using FOE_CourseRegistrationSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RegistrationSessionCourse
{
    [Key, Column(Order = 0)]
    [ForeignKey("RegistrationSession")]
    public int SessionID { get; set; }
    public RegistrationSession RegistrationSession { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Course")]
    public string CourseCode { get; set; }
    public Course Course { get; set; }

    // ✅ Pending Registrations linked to this course session
    public ICollection<PendingRegistration> PendingRegistrations { get; set; }
}

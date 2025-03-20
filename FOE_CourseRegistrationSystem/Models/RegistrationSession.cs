using FOE_CourseRegistrationSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RegistrationSession
{
    [Key]
    public int SessionID { get; set; }  // Unique ID for each registration session

    [Required]
    public string AcademicYear { get; set; }  // E.g., "2020/2021"

    [Required]
    public string Semester { get; set; }  // E.g., "1", "2", etc.

    public bool IsGeneralProgram { get; set; }  // ✅ True if it's a General Program semester (1-3)

    [ForeignKey("Department")]
    public int? DepartmentID { get; set; }  // ✅ Nullable: If General Program, no specific department needed
    public Department Department { get; set; }

    [Required]
    public DateTime StartDate { get; set; }  // Registration opening date

    [Required]
    public DateTime EndDate { get; set; }  // Registration closing date

    public bool IsOpen { get; set; }  // ✅ True = Registration Open, False = Closed

    // ✅ One RegistrationSession has multiple Courses
    public ICollection<RegistrationSessionCourse> RegistrationSessionCourses { get; set; }
}

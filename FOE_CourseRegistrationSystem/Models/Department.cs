using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FOE_CourseRegistrationSystem.Models
{
    public class Department
    {
        [Key]
        public int DepID { get; set; }

        [Required, StringLength(100)]
        public string DepartmentName { get; set; }

        [ForeignKey("HOD")]
        public int? StaffID { get; set; }
        public Staff HOD { get; set; }
    }
}

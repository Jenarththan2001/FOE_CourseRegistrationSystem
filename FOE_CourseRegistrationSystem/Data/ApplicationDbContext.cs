using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FOE_CourseRegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define database tables
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; } // Course table
        public DbSet<HasPrerequisite> HasPrerequisites { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Table Mappings
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Course>().ToTable("Course");

            // ✅ Student - Advisor (Staff) Relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(st => st.AdvisedStudents)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if an advisor exists

            // ✅ Student - Department Relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade); // Delete students if department is removed

            // ✅ Staff - Department Relationship
            modelBuilder.Entity<Staff>()
                .HasOne(st => st.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(st => st.DepID)
                .OnDelete(DeleteBehavior.Cascade); // Delete staff if department is removed

            // ✅ Department - HOD (Head of Department) Relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.HOD)
                .WithMany()
                .HasForeignKey(d => d.StaffID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of HOD while assigned

            // ✅ Course - Department Relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepID)
                .OnDelete(DeleteBehavior.Cascade); // Delete courses if department is removed

            // ✅ Define Prerequisite Relationship (Many-to-Many)
            modelBuilder.Entity<HasPrerequisite>()
                .HasKey(hp => new { hp.CourseCode, hp.PrerequisiteCode });

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Course)
                .WithMany()
                .HasForeignKey(hp => hp.CourseCode)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of a course with prerequisites

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Prerequisite)
                .WithMany()
                .HasForeignKey(hp => hp.PrerequisiteCode)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of prerequisite courses

            // ✅ CourseOffering - Course Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseCode)
                .OnDelete(DeleteBehavior.Cascade); // Delete offerings if course is removed

            // ✅ CourseOffering - Coordinator (Staff) Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Coordinator)
                .WithMany()
                .HasForeignKey(co => co.StaffID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if coordinator exists

            // ✅ Registration Relationships (Student - CourseOffering)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.NoAction);  // Prevent multiple cascade paths

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.CourseOffering)
                .WithMany()
                .HasForeignKey(r => r.OfferingID)
                .OnDelete(DeleteBehavior.NoAction);  // Prevent multiple cascade paths

            // ✅ Result - CourseOffering & Student Relationship (Composite Key)
            modelBuilder.Entity<Result>()
                .HasKey(r => new { r.OfferingID, r.StudentID });

            modelBuilder.Entity<Result>()
                .HasOne(r => r.CourseOffering)
                .WithMany()
                .HasForeignKey(r => r.OfferingID)
                .OnDelete(DeleteBehavior.Cascade); // Delete results if course offering is deleted

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of student while results exist
        }
    }
}

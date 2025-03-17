using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FOE_CourseRegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<HasPrerequisite> HasPrerequisites { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Table Mappings
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<HasPrerequisite>().ToTable("HasPrerequisite");
            modelBuilder.Entity<Registration>().ToTable("Registration");
            modelBuilder.Entity<CourseOffering>().ToTable("CourseOffering");
            modelBuilder.Entity<Result>().ToTable("Results");

            // ✅ Relationships
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(st => st.AdvisedStudents)
                .HasForeignKey(s => s.StaffID);

            modelBuilder.Entity<Staff>()
                .HasOne(d => d.Department)
                .WithMany()
                .HasForeignKey(d => d.DepID);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.HOD)
                .WithMany()
                .HasForeignKey(d => d.StaffID);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepID);

            modelBuilder.Entity<HasPrerequisite>()
                .HasKey(hp => new { hp.CourseCode, hp.PrerequisiteCode });

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Course)
                .WithMany()
                .HasForeignKey(hp => hp.CourseCode);

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Prerequisite)
                .WithMany()
                .HasForeignKey(hp => hp.PrerequisiteCode);

            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseCode);

            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Coordinator)
                .WithMany()
                .HasForeignKey(co => co.StaffID);

            modelBuilder.Entity<Result>()
                .HasKey(r => new { r.OfferingID, r.StudentID });

            modelBuilder.Entity<Result>()
                .HasOne(r => r.CourseOffering)
                .WithMany()
                .HasForeignKey(r => r.OfferingID);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID);
        }
    }
}

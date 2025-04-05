using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FOE_CourseRegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // ✅ Define database tables
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<HasPrerequisite> HasPrerequisites { get; set; }
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Result> Results { get; set; }

        // ✅ New Tables
        public DbSet<RegistrationSession> RegistrationSessions { get; set; }
        public DbSet<RegistrationSessionCourse> RegistrationSessionCourses { get; set; }
        public DbSet<PendingRegistration> PendingRegistrations { get; set; }
        public DbSet<AcademicSchedule> AcademicSchedules { get; set; }


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
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Student - Department Relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Staff - Department Relationship
            modelBuilder.Entity<Staff>()
                .HasOne(st => st.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(st => st.DepID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Department - HOD (Head of Department) Relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.HOD)
                .WithMany()
                .HasForeignKey(d => d.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Course - Department Relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Define Prerequisite Relationship (Many-to-Many)
            modelBuilder.Entity<HasPrerequisite>()
                .HasKey(hp => new { hp.CourseCode, hp.PrerequisiteCode });

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Course)
                .WithMany()
                .HasForeignKey(hp => hp.CourseCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HasPrerequisite>()
                .HasOne(hp => hp.Prerequisite)
                .WithMany()
                .HasForeignKey(hp => hp.PrerequisiteCode)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ CourseOffering - Course Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ CourseOffering - Coordinator (Staff) Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Coordinator)
                .WithMany()
                .HasForeignKey(co => co.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Registration Relationships (Student - CourseOffering)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.CourseOffering)
                .WithMany()
                .HasForeignKey(r => r.OfferingID)
                .OnDelete(DeleteBehavior.NoAction);

            // ✅ Result - CourseOffering & Student Relationship (Composite Key)
            modelBuilder.Entity<Result>()
                .HasKey(r => new { r.OfferingID, r.StudentID });

            modelBuilder.Entity<Result>()
                .HasOne(r => r.CourseOffering)
                .WithMany()
                .HasForeignKey(r => r.OfferingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ RegistrationSession - Department Relationship
            modelBuilder.Entity<RegistrationSession>()
                .HasOne(rs => rs.Department)
                .WithMany()
                .HasForeignKey(rs => rs.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ RegistrationSessionCourse - Many-to-Many Relationship
            modelBuilder.Entity<RegistrationSessionCourse>()
                .HasKey(rsc => new { rsc.SessionID, rsc.CourseCode });

            modelBuilder.Entity<RegistrationSessionCourse>()
                .HasOne(rsc => rsc.RegistrationSession)
                .WithMany(rs => rs.RegistrationSessionCourses)
                .HasForeignKey(rsc => rsc.SessionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RegistrationSessionCourse>()
                .HasOne(rsc => rsc.Course)
                .WithMany()
                .HasForeignKey(rsc => rsc.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PendingRegistration>()
       .HasKey(pr => pr.PendingID);

            modelBuilder.Entity<PendingRegistration>()
                .HasOne(pr => pr.RegistrationSessionCourse)
                .WithMany(rsc => rsc.PendingRegistrations)
                .HasForeignKey(pr => new { pr.SessionID, pr.CourseCode }) // ✅ Composite Foreign Key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PendingRegistration>()
                .HasOne(pr => pr.Student)
                .WithMany()
                .HasForeignKey(pr => pr.StudentID)
                .OnDelete(DeleteBehavior.NoAction); // ✅ Prevents multiple cascade paths

            modelBuilder.Entity<PendingRegistration>()
    .HasOne(pr => pr.CourseOffering)
    .WithMany()
    .HasForeignKey(pr => pr.CourseOfferingID)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

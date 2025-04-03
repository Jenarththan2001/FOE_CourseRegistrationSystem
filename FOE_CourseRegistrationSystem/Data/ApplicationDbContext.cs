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
        public DbSet<CourseOffering> CourseOfferings { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<RegistrationSession> RegistrationSessions { get; set; }
        public DbSet<RegistrationSessionCourse> RegistrationSessionCourses { get; set; }
        public DbSet<PendingRegistration> PendingRegistrations { get; set; }
        public DbSet<AcademicSchedule> AcademicSchedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Course>().ToTable("Course");

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(st => st.AdvisedStudents)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Staff>()
                .HasOne(st => st.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(st => st.DepID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.HOD)
                .WithMany()
                .HasForeignKey(d => d.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepID)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Coordinator)
                .WithMany()
                .HasForeignKey(co => co.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<RegistrationSession>()
                .HasOne(rs => rs.Department)
                .WithMany()
                .HasForeignKey(rs => rs.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasForeignKey(pr => new { pr.SessionID, pr.CourseCode })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PendingRegistration>()
                .HasOne(pr => pr.Student)
                .WithMany()
                .HasForeignKey(pr => pr.StudentID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PendingRegistration>()
                .HasOne(pr => pr.CourseOffering)
                .WithMany()
                .HasForeignKey(pr => pr.CourseOfferingID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>().ToTable("Notification");
        }
    }
}

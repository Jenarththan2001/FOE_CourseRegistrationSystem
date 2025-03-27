using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FOE_CourseRegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

<<<<<<< Updated upstream
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
=======
        // Database Tables
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
        public DbSet<Notification> Notifications { get; set; }
>>>>>>> Stashed changes

        public DbSet<Notification> Notifications { get; set; } // ✅ Added Notification
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
<<<<<<< Updated upstream
            // ✅ Ensure correct table name mapping
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
=======

            // Table Mappings
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Course>().ToTable("Course");
<<<<<<< Updated upstream
            modelBuilder.Entity<Notification>().ToTable("Notification");
=======
            modelBuilder.Entity<Notification>().ToTable("Notification"); // ✅ Added Notification mapping
>>>>>>> Stashed changes

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                // Configure string lengths
                entity.Property(n => n.ReceiverType).HasMaxLength(10);
                entity.Property(n => n.SenderType).HasMaxLength(10);
                entity.Property(n => n.ThirdPartyType).HasMaxLength(10);
                entity.Property(n => n.AcademicYear).HasMaxLength(9);

                // Configure message lengths (consistent with model)
                entity.Property(n => n.ReceiverMessage).IsRequired().HasMaxLength(1000);
                entity.Property(n => n.SenderMessage).HasMaxLength(1000);
                entity.Property(n => n.ThirdPartyMessage).HasMaxLength(500);

                // Add indexes for performance
                entity.HasIndex(n => n.IsRead);
                entity.HasIndex(n => n.CreatedAt);
                entity.HasIndex(n => new { n.ReceiverID, n.ReceiverType });
            });

            // Student - Advisor (Staff) Relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(st => st.AdvisedStudents)
                .HasForeignKey(s => s.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // Student - Department Relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // Staff - Department Relationship
            modelBuilder.Entity<Staff>()
                .HasOne(st => st.Department)
                .WithMany(d => d.Staffs)
                .HasForeignKey(st => st.DepID)
                .OnDelete(DeleteBehavior.Cascade);

            // Department - HOD Relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.HOD)
                .WithMany()
                .HasForeignKey(d => d.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // Course - Department Relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepID)
                .OnDelete(DeleteBehavior.Cascade);

            // Prerequisite Relationship (Many-to-Many)
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

            // CourseOffering - Course Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseCode)
                .OnDelete(DeleteBehavior.Cascade);

            // CourseOffering - Coordinator Relationship
            modelBuilder.Entity<CourseOffering>()
                .HasOne(co => co.Coordinator)
                .WithMany()
                .HasForeignKey(co => co.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // Registration Relationships
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

            // Result - Composite Key Relationship
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

            // RegistrationSession - Department Relationship
            modelBuilder.Entity<RegistrationSession>()
                .HasOne(rs => rs.Department)
                .WithMany()
                .HasForeignKey(rs => rs.DepartmentID)
                .OnDelete(DeleteBehavior.Restrict);

            // RegistrationSessionCourse - Many-to-Many Relationship
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

            // PendingRegistration Relationships
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

            // Notification Relationships (updated for disambiguated FKs)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedStudent)
                .WithMany()
                .HasForeignKey(n => n.RelatedStudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedStaff)
                .WithMany()
                .HasForeignKey(n => n.RelatedStaffID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedSession)
                .WithMany()
                .HasForeignKey(n => n.RelatedSessionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RelatedPendingRegistration)
                .WithMany()
                .HasForeignKey(n => n.RelatedPendingRegistrationID)
                .OnDelete(DeleteBehavior.Restrict);

            // Sender Relationships (disambiguated)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SenderStudent)
                .WithMany()
                .HasForeignKey(n => n.SenderStudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.SenderStaff)
                .WithMany()
                .HasForeignKey(n => n.SenderStaffID)
                .OnDelete(DeleteBehavior.Restrict);

            // Third Party Relationships (disambiguated)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ThirdPartyStudent)
                .WithMany()
                .HasForeignKey(n => n.ThirdPartyStudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ThirdPartyStaff)
                .WithMany()
                .HasForeignKey(n => n.ThirdPartyStaffID)
                .OnDelete(DeleteBehavior.Restrict);
>>>>>>> Stashed changes
        }
    }
}
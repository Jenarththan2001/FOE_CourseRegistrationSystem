using FOE_CourseRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FOE_CourseRegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ✅ Ensure correct table name mapping
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Staff>().ToTable("Staff");
        }
    }
}

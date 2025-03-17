﻿// <auto-generated />
using FOE_CourseRegistrationSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FOE_CourseRegistrationSystem.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250317221126_AddCoursesToDepartment")]
    partial class AddCoursesToDepartment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Course", b =>
                {
                    b.Property<string>("CourseCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CoreOrElective")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Credit")
                        .HasColumnType("int");

                    b.Property<int>("DepID")
                        .HasColumnType("int");

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CourseCode");

                    b.HasIndex("DepID");

                    b.ToTable("Course", (string)null);
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentID"));

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("StaffID")
                        .HasColumnType("int");

                    b.HasKey("DepartmentID");

                    b.HasIndex("StaffID");

                    b.ToTable("Department", (string)null);
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Staff", b =>
                {
                    b.Property<int>("StaffID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffID"));

                    b.Property<int>("DepID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NIC")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("StaffID");

                    b.HasIndex("DepID");

                    b.ToTable("Staff", (string)null);
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Student", b =>
                {
                    b.Property<int>("StudentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentID"));

                    b.Property<int>("AcademicYear")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NIC")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PermanentAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StaffID")
                        .HasColumnType("int");

                    b.Property<string>("TempAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StudentID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("StaffID");

                    b.ToTable("Student", (string)null);
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Course", b =>
                {
                    b.HasOne("FOE_CourseRegistrationSystem.Models.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Department", b =>
                {
                    b.HasOne("FOE_CourseRegistrationSystem.Models.Staff", "HOD")
                        .WithMany()
                        .HasForeignKey("StaffID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("HOD");
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Staff", b =>
                {
                    b.HasOne("FOE_CourseRegistrationSystem.Models.Department", "Department")
                        .WithMany("Staffs")
                        .HasForeignKey("DepID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Student", b =>
                {
                    b.HasOne("FOE_CourseRegistrationSystem.Models.Department", "Department")
                        .WithMany("Students")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FOE_CourseRegistrationSystem.Models.Staff", "Advisor")
                        .WithMany("AdvisedStudents")
                        .HasForeignKey("StaffID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Advisor");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Department", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("Staffs");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("FOE_CourseRegistrationSystem.Models.Staff", b =>
                {
                    b.Navigation("AdvisedStudents");
                });
#pragma warning restore 612, 618
        }
    }
}

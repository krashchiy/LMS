using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team45LMSContext : DbContext
    {
        public Team45LMSContext()
        {
        }

        public Team45LMSContext(DbContextOptions<Team45LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admins> Admins { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u0710248;Password=precursor;Database=Team45LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admins>(entity =>
            {
                entity.HasKey(e => new { e.AdminId, e.DeptId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptId)
                    .HasName("DeptId");

                entity.Property(e => e.AdminId).HasColumnType("char(8)");

                entity.Property(e => e.DeptId).HasColumnType("int(11)");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Admins_ibfk_1");

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Admins_ibfk_2");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.AsgCatId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassId");

                entity.HasIndex(e => new { e.Name, e.ClassId })
                    .HasName("unique_index")
                    .IsUnique();

                entity.Property(e => e.AsgCatId).HasColumnType("int(11)");

                entity.Property(e => e.ClassId).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.AsgId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AsgCatId)
                    .HasName("AsgCatId");

                entity.Property(e => e.AsgId).HasColumnType("int(11)");

                entity.Property(e => e.AsgCatId).HasColumnType("int(11)");

                entity.Property(e => e.Contents).HasColumnType("text");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.MaxPoints).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.HasOne(d => d.AsgCat)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.AsgCatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ProfessorId)
                    .HasName("ProfessorId");

                entity.HasIndex(e => new { e.CatalogId, e.Semester })
                    .HasName("unique_index")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnType("int(11)");

                entity.Property(e => e.CatalogId)
                    .IsRequired()
                    .HasColumnType("char(5)");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ProfessorId).HasColumnType("char(8)");

                entity.Property(e => e.Semester).HasColumnType("varchar(15)");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Catalog)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CatalogId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptId)
                    .HasName("DeptId");

                entity.Property(e => e.CatalogId).HasColumnType("char(5)");

                entity.Property(e => e.DeptId).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");

                entity.Property(e => e.Number).HasColumnType("char(4)");

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DeptId)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DeptId)
                    .HasName("PRIMARY");

                entity.Property(e => e.DeptId).HasColumnType("int(11)");

                entity.Property(e => e.Abbrev).HasColumnType("varchar(4)");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassId");

                entity.Property(e => e.StudentId).HasColumnType("char(8)");

                entity.Property(e => e.ClassId).HasColumnType("int(11)");

                entity.Property(e => e.Grade).HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_2");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => new { e.ProfessorId, e.DeptId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptId)
                    .HasName("DeptId");

                entity.Property(e => e.ProfessorId).HasColumnType("char(8)");

                entity.Property(e => e.DeptId).HasColumnType("int(11)");

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_2");

                entity.HasOne(d => d.Professor)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.ProfessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.DeptId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptId)
                    .HasName("DeptId");

                entity.Property(e => e.StudentId).HasColumnType("char(8)");

                entity.Property(e => e.DeptId).HasColumnType("int(11)");

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_2");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => e.SubmissionId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AsgId)
                    .HasName("AsgId");

                entity.HasIndex(e => new { e.StudentId, e.AsgId })
                    .HasName("UQ_Student_Asg")
                    .IsUnique();

                entity.Property(e => e.SubmissionId).HasColumnType("int(11)");

                entity.Property(e => e.AsgId).HasColumnType("int(11)");

                entity.Property(e => e.Contents).HasColumnType("text");

                entity.Property(e => e.Score).HasColumnType("int(11)");

                entity.Property(e => e.StudentId).HasColumnType("char(8)");

                entity.Property(e => e.TimeSubmitted).HasColumnType("datetime");

                entity.HasOne(d => d.Asg)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AsgId)
                    .HasConstraintName("Submissions_ibfk_2");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UserId).HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName).HasColumnType("varchar(100)");

                entity.Property(e => e.LastName).HasColumnType("varchar(100)");
            });
        }
    }
}

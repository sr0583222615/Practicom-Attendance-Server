using System;
using System.Collections.Generic;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataRepository
{
    public partial class Practicom_attendanceContext : DbContext
    {
        public Practicom_attendanceContext()
        {
        }

        public Practicom_attendanceContext(DbContextOptions<Practicom_attendanceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attendance> Attendances { get; set; } = null!;
        public virtual DbSet<CdPracticomType> CdPracticomTypes { get; set; } = null!;
        public virtual DbSet<CdScheduleGroupType> CdScheduleGroupTypes { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<TempCode> TempCodes { get; set; } = null!;
        public virtual DbSet<guide> Guides { get; set; } = null!;
        public virtual DbSet<practicom_guides> practicom_guides { get; set; } = null!;
        public virtual DbSet<assessments> assessments { get; set; } = null!;
        public virtual DbSet<cd_assessment_type_code> cd_assessment_type_code { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=192.116.1.21;initial catalog= practicom_attendance;user id=MbyUsers;password=1234;MultipleActiveResultSets=True");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("attendance");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DurationOfAttendance).HasColumnType("time(0)");

                entity.Property(e => e.EntryTime).HasColumnType("time(0)");

                entity.Property(e => e.ExitTime).HasColumnType("time(0)");
            });

            modelBuilder.Entity<CdPracticomType>(entity =>
            {
                entity.ToTable("cd_practicom_type");

                entity.HasIndex(e => e.Description, "UQ__cd_pract__4EBBBAC99DD904D4")
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ScheduleGroupType)
                    .WithMany(p => p.CdPracticomTypes)
                    .HasForeignKey(d => d.schedule_gruop_type_id)
                    .HasConstraintName("FK_practicom_type_schedule_group");
            });

            modelBuilder.Entity<CdScheduleGroupType>(entity =>
            {
                entity.ToTable("cd_schedule_group_type");

                entity.HasIndex(e => e.Description, "UQ__cd_sched__4EBBBAC913423C2E")
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");

                entity.HasIndex(e => new { e.Day, e.FromHour, e.ToHour, e.ScheduleGroupTypeId }, "UQ_schedule_day_hours_type")
                    .IsUnique();

                entity.Property(e => e.FromHour).HasColumnType("time(0)");

                entity.HasOne(d => d.ScheduleGroupType)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.ScheduleGroupTypeId)
                    .HasConstraintName("FK_schedule_group_type");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.HasOne(d => d.PracticomType)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.PracticomTypeId)
                    .HasConstraintName("FK_student_practicom_type");
            });

            modelBuilder.Entity<TempCode>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_TempCode");

                entity.ToTable("temp_code");

                entity.Property(e => e.DateAndTime).HasColumnType("datetime");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.TempCodes)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_temp_code_students");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

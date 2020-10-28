using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Dimension_Data_Demo.Models;

namespace Dimension_Data_Demo.Data
{
    public partial class dimention_data_demoContext : DbContext
    {
        public dimention_data_demoContext()
        {
        }

        public dimention_data_demoContext(DbContextOptions<dimention_data_demoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<CostToCompany> CostToCompany { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public virtual DbSet<EmployeeEducation> EmployeeEducation { get; set; }
        public virtual DbSet<EmployeeHistory> EmployeeHistory { get; set; }
        public virtual DbSet<EmployeePerformance> EmployeePerformance { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<JobInformation> JobInformation { get; set; }
        public virtual DbSet<MaritalStatus> MaritalStatus { get; set; }
        public virtual DbSet<Surveys> Surveys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=dimention-data-demo.cr0jdxtn9ll5.us-west-2.rds.amazonaws.com;Initial Catalog=dimention_data_demo;Persist Security Info=True;User ID=masterUsername;Password=Dd#20201023");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<CostToCompany>(entity =>
            {
                entity.HasKey(e => e.PayId);

                entity.Property(e => e.PayId).ValueGeneratedNever();

                entity.Property(e => e.OverTime).HasMaxLength(50);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeNumber);

                entity.Property(e => e.EmployeeNumber).ValueGeneratedNever();

                entity.Property(e => e.DetailsId).HasColumnName("DetailsID");

                entity.Property(e => e.EducationId).HasColumnName("EducationID");

                entity.Property(e => e.HistoryId).HasColumnName("HistoryID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.PayId).HasColumnName("PayID");

                entity.Property(e => e.PerformanceId).HasColumnName("PerformanceID");

                entity.Property(e => e.SurveyId).HasColumnName("SurveyID");

                entity.HasOne(d => d.Details)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.DetailsId)
                    .HasConstraintName("FK__Employee__Detail__52593CB8");

                entity.HasOne(d => d.Education)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.EducationId)
                    .HasConstraintName("FK__Employee__Educat__4E88ABD4");

                entity.HasOne(d => d.History)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.HistoryId)
                    .HasConstraintName("FK__Employee__Histor__4D94879B");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK__Employee__JobID__4F7CD00D");

                entity.HasOne(d => d.Pay)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.PayId)
                    .HasConstraintName("FK__Employee__PayID__49C3F6B7");

                entity.HasOne(d => d.Performance)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.PerformanceId)
                    .HasConstraintName("FK__Employee__Perfor__4CA06362");

                entity.HasOne(d => d.Survey)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.SurveyId)
                    .HasConstraintName("FK__Employee__Survey__4BAC3F29");
            });

            modelBuilder.Entity<EmployeeDetails>(entity =>
            {
                entity.HasKey(e => e.DetailsId);

                entity.Property(e => e.DetailsId)
                    .HasColumnName("DetailsID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attrition).HasMaxLength(50);

                entity.Property(e => e.GenderId).HasColumnName("GenderID");

                entity.Property(e => e.MaritalId).HasColumnName("MaritalID");

                entity.Property(e => e.Over18).HasMaxLength(50);

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.EmployeeDetails)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__EmployeeD__Gende__5165187F");

                entity.HasOne(d => d.Marital)
                    .WithMany(p => p.EmployeeDetails)
                    .HasForeignKey(d => d.MaritalId)
                    .HasConstraintName("FK__EmployeeD__Marit__5070F446");
            });

            modelBuilder.Entity<EmployeeEducation>(entity =>
            {
                entity.HasKey(e => e.EducationId);

                entity.Property(e => e.EducationId)
                    .HasColumnName("EducationID")
                    .ValueGeneratedNever();

                entity.Property(e => e.EducationField).HasMaxLength(50);
            });

            modelBuilder.Entity<EmployeeHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId);

                entity.Property(e => e.HistoryId)
                    .HasColumnName("HistoryID")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<EmployeePerformance>(entity =>
            {
                entity.HasKey(e => e.PerformanceId);

                entity.Property(e => e.PerformanceId)
                    .HasColumnName("PerformanceID")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.GenderId)
                    .HasColumnName("GenderID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Gender1)
                    .HasColumnName("Gender")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<JobInformation>(entity =>
            {
                entity.HasKey(e => e.JobId);

                entity.Property(e => e.JobId)
                    .HasColumnName("JobID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BusinessTravel).HasMaxLength(50);

                entity.Property(e => e.Department).HasMaxLength(50);

                entity.Property(e => e.JobRole).HasMaxLength(50);
            });

            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.HasKey(e => e.MaritalId);

                entity.Property(e => e.MaritalId)
                    .HasColumnName("MaritalID")
                    .ValueGeneratedNever();

                entity.Property(e => e.MaritalStatus1)
                    .HasColumnName("MaritalStatus")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Surveys>(entity =>
            {
                entity.HasKey(e => e.SurveyId);

                entity.Property(e => e.SurveyId)
                    .HasColumnName("SurveyID")
                    .ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

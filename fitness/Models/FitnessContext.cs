using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace fitness.Models;

public partial class FitnessContext : DbContext
{
    public FitnessContext()
    {
    }

    public FitnessContext(DbContextOptions<FitnessContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Abonement> Abonements { get; set; }

    public virtual DbSet<AbonementClient> AbonementClients { get; set; }

    public virtual DbSet<AdditionalService> AdditionalServices { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientInfoView> ClientInfoViews { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<HallDetail> HallDetails { get; set; }

    public virtual DbSet<HallType> HallTypes { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonScheduleView> LessonScheduleViews { get; set; }

    public virtual DbSet<LessonType> LessonTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<TrainerDetail> TrainerDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserWithRole> UserWithRoles { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    public virtual DbSet<VisitService> VisitServices { get; set; }
    public DbSet<BoolResult> BoolResults { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=fitness;Username=postgres;Password=lena123R");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<BoolResult>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });

        modelBuilder.Entity<Abonement>(entity =>
        {
            entity.HasKey(e => e.IdAbonement).HasName("abonement_pkey");

            entity.ToTable("abonement");

            entity.Property(e => e.IdAbonement).HasColumnName("id_abonement");
            entity.Property(e => e.AbonementDescription).HasColumnName("abonement_description");
            entity.Property(e => e.AbonementFreeze).HasColumnName("abonement_freeze");
            entity.Property(e => e.AbonementLong).HasColumnName("abonement_long");
            entity.Property(e => e.AbonementName)
                .HasMaxLength(20)
                .HasColumnName("abonement_name");
            entity.Property(e => e.AbonementPrice).HasColumnName("abonement_price");
        });

        modelBuilder.Entity<AbonementClient>(entity =>
        {
            entity.HasKey(e => e.IdAbonementClient).HasName("abonement_client_pkey");

            entity.ToTable("abonement_client");

            entity.Property(e => e.IdAbonementClient).HasColumnName("id_abonement_client");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.IdAbonement).HasColumnName("id_abonement");
            entity.Property(e => e.IdClient).HasColumnName("id_client");

            entity.HasOne(d => d.IdAbonementNavigation).WithMany(p => p.AbonementClients)
                .HasForeignKey(d => d.IdAbonement)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("abonement_client_id_abonement_fkey");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.AbonementClients)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("abonement_client_id_client_fkey");
        });

        modelBuilder.Entity<AdditionalService>(entity =>
        {
            entity.HasKey(e => e.IdServices).HasName("additional_services_pkey");

            entity.ToTable("additional_services");

            entity.Property(e => e.IdServices).HasColumnName("id_services");
            entity.Property(e => e.ImagePath)
                .HasColumnType("character varying")
                .HasColumnName("image_path");
            entity.Property(e => e.ServicesDescription).HasColumnName("services_description");
            entity.Property(e => e.ServicesName)
                .HasMaxLength(40)
                .HasColumnName("services_name");
            entity.Property(e => e.ServicesPrice).HasColumnName("services_price");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Clients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("client_user_id_fkey");
        });

        modelBuilder.Entity<ClientInfoView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("client_info_view");

            entity.Property(e => e.AbonementEndDate).HasColumnName("abonement_end_date");
            entity.Property(e => e.AbonementName)
                .HasMaxLength(20)
                .HasColumnName("abonement_name");
            entity.Property(e => e.AbonementStartDate).HasColumnName("abonement_start_date");
            entity.Property(e => e.AbonementStatus).HasColumnName("abonement_status");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DaysRemaining).HasColumnName("days_remaining");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("first_name");
            entity.Property(e => e.IdAbonementClient).HasColumnName("id_abonement_client");
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(60)
                .HasColumnName("middle_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.IdHall).HasName("hall_pkey");

            entity.ToTable("hall");

            entity.Property(e => e.IdHall).HasColumnName("id_hall");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.CapacityHall).HasColumnName("capacity_hall");
            entity.Property(e => e.IdTypeHall).HasColumnName("id_type_hall");
            entity.Property(e => e.ImagePath)
                .HasColumnType("character varying")
                .HasColumnName("image_path");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasOne(d => d.IdTypeHallNavigation).WithMany(p => p.Halls)
                .HasForeignKey(d => d.IdTypeHall)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hall_id_type_hall_fkey");
        });

        modelBuilder.Entity<HallDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("hall_details");

            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.HallType)
                .HasMaxLength(30)
                .HasColumnName("hall_type");
            entity.Property(e => e.IdHall).HasColumnName("id_hall");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.TypeDescription).HasColumnName("type_description");
        });

        modelBuilder.Entity<HallType>(entity =>
        {
            entity.HasKey(e => e.IdTypeHall).HasName("hall_type_pkey");

            entity.ToTable("hall_type");

            entity.Property(e => e.IdTypeHall).HasColumnName("id_type_hall");
            entity.Property(e => e.NameTypeHall)
                .HasMaxLength(30)
                .HasColumnName("name_type_hall");
            entity.Property(e => e.TypeHallDesc).HasColumnName("type_hall_desc");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.IdLesson).HasName("lesson_pkey");

            entity.ToTable("lesson");

            entity.Property(e => e.IdLesson).HasColumnName("id_lesson");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IdAbonementClient).HasColumnName("id_abonement_client");
            entity.Property(e => e.IdHall).HasColumnName("id_hall");
            entity.Property(e => e.IdTrainer).HasColumnName("id_trainer");
            entity.Property(e => e.IdTypeLesson).HasColumnName("id_type_lesson");
            entity.Property(e => e.LessonDate).HasColumnName("lesson_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.IdAbonementClientNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdAbonementClient)
                .HasConstraintName("lesson_id_abonement_client_fkey");

            entity.HasOne(d => d.IdHallNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdHall)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lesson_id_hall_fkey");

            entity.HasOne(d => d.IdTrainerNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdTrainer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lesson_id_trainer_fkey");

            entity.HasOne(d => d.IdTypeLessonNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.IdTypeLesson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lesson_id_type_lesson_fkey");
        });

        modelBuilder.Entity<LessonScheduleView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("lesson_schedule_view");

            entity.Property(e => e.IdAbonement).HasColumnName("id_abonement");
            entity.Property(e => e.IdLesson).HasColumnName("id_lesson");
            entity.Property(e => e.LessonDate).HasColumnName("lesson_date");
            entity.Property(e => e.LessonEnd).HasColumnName("lesson_end");
            entity.Property(e => e.LessonStart).HasColumnName("lesson_start");
            entity.Property(e => e.LessonTypeName)
                .HasMaxLength(255)
                .HasColumnName("lesson_type_name");
            entity.Property(e => e.TrainerFullName).HasColumnName("trainer_full_name");
            entity.Property(e => e.TypeHallName)
                .HasMaxLength(30)
                .HasColumnName("type_hall_name");
        });

        modelBuilder.Entity<LessonType>(entity =>
        {
            entity.HasKey(e => e.IdTypeLesson).HasName("lesson_type_pkey");

            entity.ToTable("lesson_type");

            entity.Property(e => e.IdTypeLesson).HasColumnName("id_type_lesson");
            entity.Property(e => e.TypeLessonDesc).HasColumnName("type_lesson_desc");
            entity.Property(e => e.TypeLessonName)
                .HasMaxLength(255)
                .HasColumnName("type_lesson_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Name, "role_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.IdTrainer).HasName("trainer_pkey");

            entity.ToTable("trainer");

            entity.Property(e => e.IdTrainer).HasColumnName("id_trainer");
            entity.Property(e => e.TrainerEducation)
                .HasMaxLength(60)
                .HasColumnName("trainer_education");
            entity.Property(e => e.TrainerExperiance).HasColumnName("trainer_experiance");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Trainers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("trainer_user_id_fkey");
        });

        modelBuilder.Entity<TrainerDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("trainer_details");

            entity.Property(e => e.AgeUser).HasColumnName("age_user");
            entity.Property(e => e.Education)
                .HasMaxLength(60)
                .HasColumnName("education");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("first_name");
            entity.Property(e => e.IdTrainer).HasColumnName("id_trainer");
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(60)
                .HasColumnName("middle_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AgeUser).HasColumnName("age_user");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastnameUser)
                .HasMaxLength(60)
                .HasColumnName("lastname_user");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(60)
                .HasColumnName("middle_name");
            entity.Property(e => e.NameUser)
                .HasMaxLength(40)
                .HasColumnName("name_user");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserWithRole>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("user_with_role");

            entity.Property(e => e.AgeUser).HasColumnName("age_user");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(40)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(60)
                .HasColumnName("middle_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.IdVisit).HasName("visit_pkey");

            entity.ToTable("visit");

            entity.Property(e => e.IdVisit).HasColumnName("id_visit");
            entity.Property(e => e.DateTime).HasColumnName("date_time");
            entity.Property(e => e.DateVisit).HasColumnName("date_visit");
            entity.Property(e => e.IdAbonementClient).HasColumnName("id_abonement_client");

            entity.HasOne(d => d.IdAbonementClientNavigation).WithMany(p => p.Visits)
                .HasForeignKey(d => d.IdAbonementClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("visit_id_abonement_client_fkey");
        });

        modelBuilder.Entity<VisitService>(entity =>
        {
            entity.HasKey(e => e.IdVisitServices).HasName("visit_services_pkey");

            entity.ToTable("visit_services");

            entity.Property(e => e.IdVisitServices).HasColumnName("id_visit_services");
            entity.Property(e => e.IdServices).HasColumnName("id_services");
            entity.Property(e => e.IdTime).HasColumnName("id_time");
            entity.Property(e => e.IdVisit).HasColumnName("id_visit");

            entity.HasOne(d => d.IdServicesNavigation).WithMany(p => p.VisitServices)
                .HasForeignKey(d => d.IdServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("visit_services_id_services_fkey");

            entity.HasOne(d => d.IdVisitNavigation).WithMany(p => p.VisitServices)
                .HasForeignKey(d => d.IdVisit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("visit_services_id_visit_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using AttStat.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace AttStat;

public partial class SystemDataContext : DbContext
{
    public bool IsDisposed { get; set; }
    public SystemDataContext()
    {
    }

    public SystemDataContext(DbContextOptions<SystemDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attestation> Attestations { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<EducationalCategory> EducationalCategories { get; set; }

    public virtual DbSet<EducationalForm> EducationalForms { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseSqlite("Data Source=data\\SystemData.db")
            .UseLazyLoadingProxies();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attestation>(entity =>
        {
            entity.ToTable("attestation");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Discipline).HasColumnName("discipline");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Student).HasColumnName("student");

            entity.HasOne(d => d.DisciplineNavigation).WithMany(p => p.Attestations)
                .HasForeignKey(d => d.Discipline)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StudentNavigation).WithMany(p => p.Attestations)
                .HasForeignKey(d => d.Student)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.ToTable("disciplines");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.Name)
                .HasColumnType("INTEGER")
                .HasColumnName("name");
            entity.Property(e => e.Specialization).HasColumnName("specialization");

            entity.HasOne(d => d.SpecializationNavigation).WithMany(p => p.Disciplines).HasForeignKey(d => d.Specialization);
        });

        modelBuilder.Entity<EducationalCategory>(entity =>
        {
            entity.ToTable("educational_categories");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<EducationalForm>(entity =>
        {
            entity.ToTable("educational_forms");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("faculties");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("groups");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Course).HasColumnName("course");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Specialization).HasColumnName("specialization");

            entity.HasOne(d => d.SpecializationNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.Specialization)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.ToTable("specializations");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Faculty).HasColumnName("faculty");
            entity.Property(e => e.Name)
                .HasColumnType("INTEGER")
                .HasColumnName("name");

            entity.HasOne(d => d.FacultyNavigation).WithMany(p => p.Specializations).HasForeignKey(d => d.Faculty);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");

            entity.HasIndex(e => e.CardId, "IX_students_card_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.EducationCategory).HasColumnName("education_category");
            entity.Property(e => e.EducationForm).HasColumnName("education_form");
            entity.Property(e => e.EnrollmentYear).HasColumnName("enrollment_year");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.Group).HasColumnName("group");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.MiddleName).HasColumnName("middle_name");

            entity.HasOne(d => d.EducationCategoryNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationCategory)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EducationFormNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationForm)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.GroupNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.Group)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    public override void Dispose()
    {
        IsDisposed = true;
        base.Dispose();
    }
}

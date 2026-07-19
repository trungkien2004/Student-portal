using Microsoft.EntityFrameworkCore;
using StudentPortal.API.Models;

namespace StudentPortal.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Major> Majors => Set<Major>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<FamilyContact> FamilyContacts => Set<FamilyContact>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<CurriculumItem> CurriculumItems => Set<CurriculumItem>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<ConductScore> ConductScores => Set<ConductScore>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Tuition> Tuitions => Set<Tuition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FamilyContact>()
            .HasKey(f => f.ContactId);

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentCode)
            .IsUnique();

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.SubjectCode)
            .IsUnique();

        modelBuilder.Entity<Grade>()
            .Property(g => g.MidtermLT).HasColumnType("decimal(4,2)");
        modelBuilder.Entity<Grade>()
            .Property(g => g.MidtermTH).HasColumnType("decimal(4,2)");
        modelBuilder.Entity<Grade>()
            .Property(g => g.FinalLT).HasColumnType("decimal(4,2)");
        modelBuilder.Entity<Grade>()
            .Property(g => g.FinalTH).HasColumnType("decimal(4,2)");
        modelBuilder.Entity<Grade>()
            .Property(g => g.AverageScore).HasColumnType("decimal(4,2)");

        modelBuilder.Entity<Tuition>()
            .Property(t => t.AmountDue).HasColumnType("decimal(12,0)");
        modelBuilder.Entity<Tuition>()
            .Property(t => t.AmountPaid).HasColumnType("decimal(12,0)");

        base.OnModelCreating(modelBuilder);
    }
}

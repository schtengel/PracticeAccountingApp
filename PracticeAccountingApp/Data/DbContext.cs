using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;

namespace PracticeAccountingApp.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<PracticeSheet> PracticeSheets { get; set; }

    public virtual DbSet<PracticeType> PracticeTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentReport> StudentReports { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PracticeAccounting;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupNumber).HasName("PK__Groups__8BA4751B7804E9EF");

            entity.Property(e => e.GroupNumber).HasMaxLength(8);
            entity.Property(e => e.Specialization).HasMaxLength(200);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B7477875270DB37");

            entity.Property(e => e.ModuleId)
                .HasMaxLength(10)
                .HasColumnName("ModuleID");
            entity.Property(e => e.ModuleName).HasMaxLength(200);
        });

        modelBuilder.Entity<PracticeSheet>(entity =>
        {
            entity.HasKey(e => e.PracticeSheetId).HasName("PK__Practice__AA44124AA0D9583F");

            entity.Property(e => e.PracticeSheetId).HasColumnName("PracticeSheetID");
            entity.Property(e => e.GroupNumber).HasMaxLength(8);
            entity.Property(e => e.ModuleId)
                .HasMaxLength(10)
                .HasColumnName("ModuleID");
            entity.Property(e => e.PracticeTypeId).HasColumnName("PracticeTypeID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.GroupNumberNavigation).WithMany(p => p.PracticeSheets)
                .HasForeignKey(d => d.GroupNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PracticeSheets_Groups");

            entity.HasOne(d => d.Module).WithMany(p => p.PracticeSheets)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PracticeSheets_Modules");

            entity.HasOne(d => d.PracticeType).WithMany(p => p.PracticeSheets)
                .HasForeignKey(d => d.PracticeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PracticeSheets_PracticeTypes");

            entity.HasOne(d => d.Teacher).WithMany(p => p.PracticeSheets)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PracticeSheets_Teachers");
        });

        modelBuilder.Entity<PracticeType>(entity =>
        {
            entity.HasKey(e => e.PracticeTypeId).HasName("PK__Practice__06B4D866E775D209");

            entity.Property(e => e.PracticeTypeId).HasColumnName("PracticeTypeID");
            entity.Property(e => e.PracticeTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A7F1A22AB");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79540EE8C7");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.GroupNumber).HasMaxLength(8);

            entity.HasOne(d => d.GroupNumberNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Groups");
        });

        modelBuilder.Entity<StudentReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__StudentR__D5BD48E596F710B4");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.PracticeSheetId).HasColumnName("PracticeSheetID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.PracticeSheet).WithMany(p => p.StudentReports)
                .HasForeignKey(d => d.PracticeSheetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentReports_PracticeSheets");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentReports)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentReports_Students");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF259449725A8AC");

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.FullName).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC798B5EFB");

            entity.HasIndex(e => e.Login, "UQ__Users__5E55825B8F13594D").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

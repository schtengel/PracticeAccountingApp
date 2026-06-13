using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;

namespace PracticeAccountingApp.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
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
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PracticeAccounting;TrustServerCertificate=true;");
        }
    }

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

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Администратор" },
            new Role { RoleId = 2, RoleName = "Преподаватель" },
            new Role { RoleId = 3, RoleName = "Пользователь" }
        );

        modelBuilder.Entity<PracticeType>().HasData(
        new PracticeType { PracticeTypeId = 1, PracticeTypeName = "Учебная" },
        new PracticeType { PracticeTypeId = 2, PracticeTypeName = "Производственная" },
        new PracticeType { PracticeTypeId = 3, PracticeTypeName = "Преддипломная" }
    );

        modelBuilder.Entity<User>().HasData(
        new User
        {
            UserId = 1,
            Login = "admin",
            PasswordHash = "admin", // Здесь должен быть реальный хэш
            RoleId = 1
        }
    );

        modelBuilder.Entity<Module>().HasData(
    new Module { ModuleId = "MOD-EL01", ModuleName = "Электротехника и электроника" },
    new Module { ModuleId = "MOD-EL02", ModuleName = "Электрические машины и аппараты" },
    new Module { ModuleId = "MOD-EL03", ModuleName = "Релейная защита и автоматика" },
    new Module { ModuleId = "MOD-EL04", ModuleName = "Электроснабжение предприятий" },
    new Module { ModuleId = "MOD-G01", ModuleName = "Математика" },
    new Module { ModuleId = "MOD-G02", ModuleName = "Информатика" },
    new Module { ModuleId = "MOD-G03", ModuleName = "Английский язык в профессиональной деятельности" },
    new Module { ModuleId = "MOD-G04", ModuleName = "Экономика организации" },
    new Module { ModuleId = "MOD-G05", ModuleName = "Безопасность жизнедеятельности" },
    new Module { ModuleId = "MOD-G06", ModuleName = "Правовое обеспечение профессиональной деятельности" },
    new Module { ModuleId = "MOD-IT01", ModuleName = "Основы программирования" },
    new Module { ModuleId = "MOD-IT02", ModuleName = "Объектно-ориентированное программирование" },
    new Module { ModuleId = "MOD-IT03", ModuleName = "Базы данных и SQL" },
    new Module { ModuleId = "MOD-IT04", ModuleName = "Веб-разработка (HTML, CSS, JavaScript)" },
    new Module { ModuleId = "MOD-IT05", ModuleName = "Разработка backend-приложений (C#)" },
    new Module { ModuleId = "MOD-IT06", ModuleName = "Веб-фреймворки и серверная разработка" },
    new Module { ModuleId = "MOD-IT07", ModuleName = "Разработка мобильных приложений" },
    new Module { ModuleId = "MOD-IT08", ModuleName = "Тестирование и отладка ПО" },
    new Module { ModuleId = "MOD-MS01", ModuleName = "Технология машиностроения" },
    new Module { ModuleId = "MOD-MS02", ModuleName = "Материаловедение" },
    new Module { ModuleId = "MOD-MS03", ModuleName = "Черчение и инженерная графика" },
    new Module { ModuleId = "MOD-MS04", ModuleName = "Метрология, стандартизация и сертификация" },
    new Module { ModuleId = "MOD-MS05", ModuleName = "Автоматизация технологических процессов" },
    new Module { ModuleId = "MOD-MS06", ModuleName = "Робототехника и мехатроника" },
    new Module { ModuleId = "MOD-SV01", ModuleName = "Сварочное дело" },
    new Module { ModuleId = "MOD-SV02", ModuleName = "Оборудование и технология сварки" },
    new Module { ModuleId = "MOD-SV03", ModuleName = "Контроль качества сварных соединений" }
);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

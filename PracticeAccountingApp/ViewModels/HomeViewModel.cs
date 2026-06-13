using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace PracticeAccountingApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    [ObservableProperty] private int groupsCount;
    [ObservableProperty] private int studentsCount;
    [ObservableProperty] private int practicesCount;
    [ObservableProperty] private int reportsCount;

    public ObservableCollection<StatementCardVm> LastStatements { get; } = new();
    public ObservableCollection<string> LastActions { get; } = new();
    public ObservableCollection<PracticeCardVm> UpcomingPractices { get; } = new();

    public HomeViewModel()
    {
        LoadData();
    }

    private void LoadData()
    {
        GroupsCount = Db.Context.Groups.Count();
        StudentsCount = Db.Context.Students.Count();
        PracticesCount = Db.Context.PracticeSheets.Count();
        ReportsCount = Db.Context.StudentReports.Count();

        LastStatements.Clear();
        UpcomingPractices.Clear();
        LastActions.Clear();

        // -------------------------
        // 1. Последние ведомости → карточки
        // -------------------------
        var lastSheets = Db.Context.PracticeSheets
            .Include(s => s.PracticeType)
            .Include(s => s.Teacher)
            .OrderByDescending(x => x.PracticeSheetId)
            .Take(6)
            .AsNoTracking()
            .ToList();

        foreach (var sheet in lastSheets)
        {
            LastStatements.Add(new StatementCardVm
            {
                GroupName = sheet.GroupNumber,
                PracticeName = sheet.PracticeType?.PracticeTypeName ?? "—",
                TeacherName = sheet.Teacher?.FullName ?? "—",
                Date = sheet.StartDate
            });
        }

        // -------------------------
        // 2. Ближайшие практики (1 месяц вперёд)
        // -------------------------
        var now = DateOnly.FromDateTime(DateTime.Now);
        var monthAhead = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

        var upcoming = Db.Context.PracticeSheets
            .Include(p => p.PracticeType)
            .Where(x => x.StartDate >= now && x.StartDate <= monthAhead)
            .OrderBy(x => x.StartDate)
            .Take(6)
            .AsNoTracking()
            .ToList();

        foreach (var p in upcoming)
        {
            UpcomingPractices.Add(new PracticeCardVm
            {
                Group = p.GroupNumber,
                PracticeName = p.PracticeType?.PracticeTypeName ?? "—",
                StartDate = p.StartDate
            });
        }

        // -------------------------
        // 3. Последние действия — объединяем студентов и ведомости в один
        //    список и сортируем по ID (прокси для времени создания).
        //    Тип действия кодируем в знаке: студенты → чётные, ведомости → нечётные,
        //    затем сортируем по убыванию суррогатного ключа.
        // -------------------------
        var recentStudents = Db.Context.Students
            .OrderByDescending(x => x.StudentId)
            .Take(5)
            .Select(s => new ActionItem
            {
                SortKey = s.StudentId * 2,      // чётный суррогат
                Text = $"Добавлен студент: {s.FullName}"
            })
            .AsNoTracking()
            .ToList();

        var recentSheets = Db.Context.PracticeSheets
            .Include(s => s.PracticeType)
            .OrderByDescending(x => x.PracticeSheetId)
            .Take(5)
            .AsNoTracking()
            .ToList()
            .Select(s => new ActionItem
            {
                SortKey = s.PracticeSheetId * 2 + 1,   // нечётный суррогат
                Text = $"Создана ведомость: {s.PracticeType?.PracticeTypeName ?? "—"} ({s.GroupNumber})"
            });

        // Объединяем и берём 6 самых свежих записей
        var combined = recentStudents
            .Concat(recentSheets)
            .OrderByDescending(x => x.SortKey)
            .Take(6);

        foreach (var action in combined)
            LastActions.Add(action.Text);
    }

    // Вспомогательный тип — не нужен снаружи, поэтому приватный
    private class ActionItem
    {
        public int SortKey { get; init; }
        public string Text { get; init; } = "";
    }
}

public class StatementCardVm
{
    public string GroupName { get; set; } = "";
    public string PracticeName { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public DateOnly Date { get; set; }
}

public class PracticeCardVm
{
    public string Group { get; set; } = "";
    public string PracticeName { get; set; } = "";
    public DateOnly StartDate { get; set; }
}
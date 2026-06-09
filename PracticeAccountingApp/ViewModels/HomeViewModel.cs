using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;
using System;
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
            .OrderByDescending(x => x.PracticeSheetId)
            .Take(6)
            .ToList();

        foreach (var sheet in lastSheets)
        {
            LastStatements.Add(new StatementCardVm
            {
                GroupName = sheet.GroupNumber,
                PracticeName = sheet.PracticeType.PracticeTypeName,
                TeacherName = sheet.Teacher.FullName,
                Date = sheet.StartDate
            });
        }

        // -------------------------
        // 2. Ближайшие практики (1 месяц вперёд)
        // -------------------------
        var now = DateOnly.FromDateTime(DateTime.Now);
        var monthAhead = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

        var upcoming = Db.Context.PracticeSheets
            .Where(x => x.StartDate >= now && x.StartDate <= monthAhead)
            .OrderBy(x => x.StartDate)
            .Take(6)
            .ToList();

        foreach (var p in upcoming)
        {
            UpcomingPractices.Add(new PracticeCardVm
            {
                Group = p.GroupNumber,
                PracticeName = p.PracticeType.PracticeTypeName,
                StartDate = p.StartDate
            });
        }

        // -------------------------
        // 3. Реальные "последние действия"
        // -------------------------
        var lastStudents = Db.Context.Students
            .OrderByDescending(x => x.StudentId)
            .Take(3)
            .ToList();

        foreach (var s in lastStudents)
        {
            LastActions.Add($"Добавлен студент: {s.FullName}");
        }

        var lastSheetsShort = Db.Context.PracticeSheets
            .OrderByDescending(x => x.PracticeSheetId)
            .Take(3)
            .ToList();

        foreach (var s in lastSheetsShort)
        {
            LastActions.Add($"Создана ведомость: {s.PracticeType.PracticeTypeName}");
        }
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
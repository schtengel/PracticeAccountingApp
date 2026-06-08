using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;

namespace PracticeAccountingApp.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    [ObservableProperty]
    private int groupsCount;

    [ObservableProperty]
    private int studentsCount;

    [ObservableProperty]
    private int practicesCount;

    [ObservableProperty]
    private int reportsCount;

    public ObservableCollection<StatementInfo> LastStatements { get; } = [];

    public ObservableCollection<string> UpcomingPractices { get; } = [];

    public ObservableCollection<string> LastActions { get; } = [];

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

        var sheets = Db.Context.PracticeSheets
            .OrderByDescending(x => x.PracticeSheetId)
            .Take(10)
            .ToList();

        foreach (var sheet in sheets)
        {
            LastStatements.Add(new StatementInfo
            {
                GroupName = sheet.GroupNumber,
                PracticeName = sheet.PracticeType.PracticeTypeName,
                TeacherName = sheet.Teacher.FullName,
                StartDate = sheet.StartDate,
                EndDate = sheet.EndDate
            });
        }

        var upcoming = Db.Context.PracticeSheets
            .OrderBy(x => x.StartDate)
            .Take(5)
            .ToList();

        foreach (var practice in upcoming)
        {
            UpcomingPractices.Add(
                $"{practice.GroupNumber} — {practice.PracticeType.PracticeTypeName}");
        }

        LastActions.Add($"Всего студентов: {StudentsCount}");
        LastActions.Add($"Всего групп: {GroupsCount}");
        LastActions.Add($"Всего ведомостей: {PracticesCount}");
    }
}
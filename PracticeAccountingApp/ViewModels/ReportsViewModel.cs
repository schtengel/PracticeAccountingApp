using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PracticeAccountingApp.ViewModels;

public partial class ReportsViewModel : BaseViewModel
{
    private readonly DispatcherTimer _searchTimer;

    [ObservableProperty]
    private string searchText = "";

    public ObservableCollection<ReportVm> Reports { get; } = new();

    public ReportsViewModel()
    {
        _searchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); Load(); };
        Load();
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchTimer.Stop();
        _searchTimer.Start();
    }

    public void Load()
    {
        Reports.Clear();

        var query = Db.Context.StudentReports
            .Include(r => r.Student)
            .Include(r => r.PracticeSheet)
                .ThenInclude(ps => ps.PracticeType)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(r =>
                r.Student.FullName.Contains(SearchText) ||
                r.Student.GroupNumber.Contains(SearchText) ||
                r.PracticeSheet.PracticeType.PracticeTypeName.Contains(SearchText));
        }

        var data = query
            .OrderByDescending(r => r.ReportId)
            .Select(r => new ReportVm
            {
                Id = r.ReportId,
                StudentName = r.Student.FullName,
                Group = r.Student.GroupNumber,
                PracticeType = r.PracticeSheet.PracticeType.PracticeTypeName,
                Grade = r.Grade,
                SubmissionDate = r.SubmissionDate,
                FilePath = r.FilePath
            })
            .ToList();

        foreach (var item in data)
            Reports.Add(item);
    }

    [RelayCommand]
    public void OpenAdd()
    {
        var win = new StudentReportEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Edit(ReportVm vm)
    {
        if (vm == null) return;
        var win = new StudentReportEditWindow(vm.Id);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Delete(ReportVm vm)
    {
        if (vm == null) return;

        if (MessageBox.Show(
                $"Удалить отчёт студента {vm.StudentName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
            != MessageBoxResult.Yes)
            return;

        try
        {
            var entity = Db.Context.StudentReports
                .FirstOrDefault(r => r.ReportId == vm.Id);
            if (entity == null) return;

            Db.Context.StudentReports.Remove(entity);
            Db.Context.SaveChanges();
            Reports.Remove(vm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // CanManagePractices унаследован из BaseViewModel
}

public class ReportVm
{
    public int Id { get; set; }
    public string StudentName { get; set; } = "";
    public string Group { get; set; } = "";
    public string PracticeType { get; set; } = "";
    public byte? Grade { get; set; }
    public DateOnly? SubmissionDate { get; set; }
    public string FilePath { get; set; } = "";

    public string GradeDisplay => Grade.HasValue ? Grade.Value.ToString() : "—";

    public string GradeColor => Grade switch
    {
        5 => "#2E7D32",
        4 => "#1565C0",
        3 => "#E65100",
        2 => "#C62828",
        _ => "#5F6368"
    };
}
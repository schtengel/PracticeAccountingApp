using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticesViewModel : BaseViewModel
{
    private readonly DispatcherTimer _searchTimer;

    [ObservableProperty]
    private string searchText = "";

    public ObservableCollection<PracticeVm> Practices { get; } = new();

    public PracticesViewModel()
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

    private void Load()
    {
        Practices.Clear();

        var query = Db.Context.PracticeSheets
            .Include(p => p.PracticeType)
            .Include(p => p.Module)
            .Include(p => p.Teacher)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(p =>
                p.PracticeType.PracticeTypeName.Contains(SearchText) ||
                p.Module.ModuleName.Contains(SearchText) ||
                p.Teacher.FullName.Contains(SearchText) ||
                p.GroupNumber.Contains(SearchText));
        }

        var data = query
            .Select(p => new PracticeVm
            {
                Id = p.PracticeSheetId,
                PracticeType = p.PracticeType.PracticeTypeName,
                ModuleName = p.Module.ModuleName,
                TeacherName = p.Teacher.FullName,
                Group = p.GroupNumber,
                Start = p.StartDate,
                End = p.EndDate
            })
            .ToList();

        foreach (var item in data)
            Practices.Add(item);
    }

    [RelayCommand]
    public void OpenAdd()
    {
        var win = new PracticeEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Edit(PracticeVm vm)
    {
        if (vm == null) return;

        var win = new PracticeEditWindow(vm.Id);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Delete(PracticeVm vm)
    {
        if (vm == null) return;

        if (MessageBox.Show(
                $"Удалить ведомость для группы {vm.Group}?\nВсе связанные отчёты студентов также будут удалены.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        try
        {
            var entity = Db.Context.PracticeSheets
                .FirstOrDefault(x => x.PracticeSheetId == vm.Id);

            if (entity == null) return;

            // Сначала удаляем дочерние записи — без этого вылетит FK-исключение,
            // если в БД не настроен CASCADE DELETE.
            var reports = Db.Context.StudentReports
                .Where(r => r.PracticeSheetId == vm.Id)
                .ToList();

            Db.Context.StudentReports.RemoveRange(reports);
            Db.Context.PracticeSheets.Remove(entity);
            Db.Context.SaveChanges();

            Practices.Remove(vm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // CanManagePractices унаследован из BaseViewModel — дублирование устранено.
}

public class PracticeVm
{
    public int Id { get; set; }
    public string PracticeType { get; set; } = "";
    public string ModuleName { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public string Group { get; set; } = "";
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}
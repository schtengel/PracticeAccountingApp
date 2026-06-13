using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PracticeAccountingApp.ViewModels;

public partial class StudentsViewModel : BaseViewModel
{
    private readonly DispatcherTimer _searchTimer;

    [ObservableProperty]
    private string searchText = "";

    public ObservableCollection<StudentVm> Students { get; } = new();

    public StudentsViewModel()
    {
        _searchTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };

        _searchTimer.Tick += (_, _) =>
        {
            _searchTimer.Stop();
            Load();
        };

        Load();
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchTimer.Stop();
        _searchTimer.Start();
    }

    private void Load()
    {
        Students.Clear();

        var query = Db.Context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            // Поиск и по ФИО, и по номеру группы
            query = query.Where(s =>
                s.FullName.Contains(SearchText) ||
                s.GroupNumber.Contains(SearchText));
        }

        var data = query
            .Select(s => new StudentVm
            {
                Id = s.StudentId,
                FullName = s.FullName,
                Group = s.GroupNumber,
                BirthDate = s.BirthDate
            })
            .OrderBy(s => s.FullName)
            .ToList();

        foreach (var item in data)
            Students.Add(item);
    }

    [RelayCommand]
    private void Delete(StudentVm student)
    {
        if (student == null) return;

        var entity = Db.Context.Students
            .FirstOrDefault(x => x.StudentId == student.Id);

        if (entity == null) return;

        // 1. проверка зависимостей
        var reportsCount = Db.Context.StudentReports
            .Count(r => r.StudentId == student.Id);

        if (MessageBox.Show(
                $"Удалить студента {student.FullName}?\n" +
                $"Найдено отчетов: {reportsCount}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        // 2. если есть отчеты — второе подтверждение
        if (reportsCount > 0)
        {
            var secondConfirm = MessageBox.Show(
                $"У студента есть {reportsCount} отчетов.\n" +
                "Они будут УДАЛЕНЫ вместе со студентом.\n\nПродолжить?",
                "Второе подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            if (secondConfirm != MessageBoxResult.Yes)
                return;

            // удаляем отчеты
            var reports = Db.Context.StudentReports
                .Where(r => r.StudentId == student.Id);

            Db.Context.StudentReports.RemoveRange(reports);
        }

        try
        {
            Db.Context.Students.Remove(entity);
            Db.Context.SaveChanges();

            Students.Remove(student);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка удаления",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void OpenAdd()
    {
        var win = new StudentEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    private void Edit(StudentVm student)
    {
        var win = new StudentEditWindow(student.Id);
        win.ShowDialog();
        Load();
    }

    // CanManageGroupsAndStudents и CanManagePractices унаследованы из BaseViewModel.
}

public class StudentVm
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Group { get; set; } = "";
    public DateOnly BirthDate { get; set; }
}
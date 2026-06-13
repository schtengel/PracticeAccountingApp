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

        if (MessageBox.Show(
                $"Удалить студента {student.FullName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        try
        {
            var entity = Db.Context.Students
                .FirstOrDefault(x => x.StudentId == student.Id);

            if (entity == null) return;

            Db.Context.Students.Remove(entity);
            Db.Context.SaveChanges();

            Students.Remove(student);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
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
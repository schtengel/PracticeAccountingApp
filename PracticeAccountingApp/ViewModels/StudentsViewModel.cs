using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
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

        _searchTimer.Tick += (_, __) =>
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
            query = query.Where(s => s.FullName.Contains(SearchText));
        }

        var data = query
            .Select(s => new StudentVm
            {
                Id = s.StudentId,
                FullName = s.FullName,
                Group = s.GroupNumber,
                BirthDate = s.BirthDate
            })
            .ToList();

        foreach (var item in data)
            Students.Add(item);
    }

    [RelayCommand]
    private void Delete(StudentVm student)
    {
        var entity = Db.Context.Students
            .FirstOrDefault(x => x.StudentId == student.Id);

        if (entity == null) return;

        Db.Context.Students.Remove(entity);
        Db.Context.SaveChanges();

        Students.Remove(student);
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

    // Права доступа
    public bool CanManageGroupsAndStudents =>
        App.Current.MainWindow?.DataContext is MainViewModel mvm && mvm.CanManageGroupsAndStudents;

    public bool CanManagePractices =>
        App.Current.MainWindow?.DataContext is MainViewModel mvm && mvm.CanManagePractices;
}

public class StudentVm
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Group { get; set; } = "";
    public DateOnly BirthDate { get; set; }
}
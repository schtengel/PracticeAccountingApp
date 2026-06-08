using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class StudentsViewModel : BaseViewModel
{
    [ObservableProperty]
    private string searchText = "";

    public ObservableCollection<StudentVm> Students { get; } = new();

    public StudentsViewModel()
    {
        Load();
    }

    private void Load()
    {
        Students.Clear();

        var data = Db.Context.Students
            .Where(s => string.IsNullOrWhiteSpace(SearchText)
                        || s.FullName.Contains(SearchText))
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
    private void Search()
    {
        Load();
    }

    [RelayCommand]
    private void Delete(StudentVm student)
    {
        var entity = Db.Context.Students.FirstOrDefault(x => x.StudentId == student.Id);

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
}

public class StudentVm
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Group { get; set; } = "";
    public DateOnly BirthDate { get; set; }
}
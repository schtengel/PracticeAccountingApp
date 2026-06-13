using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class StudentEditViewModel : BaseViewModel
{
    private readonly int? _id;

    [ObservableProperty]
    private string fullName = "";

    [ObservableProperty]
    private string group = "";

    [ObservableProperty]
    private string birthDateString = "";

    public ObservableCollection<string> Groups { get; } = new();

    public StudentEditViewModel(int? id)
    {
        _id = id;
        LoadGroups();

        if (id != null)
        {
            var s = Db.Context.Students.First(x => x.StudentId == id);

            FullName = s.FullName;
            Group = s.GroupNumber;
            BirthDateString = s.BirthDate.ToString("dd.MM.yyyy");
        }
    }

    private void LoadGroups()
    {
        Groups.Clear();

        var groups = Db.Context.Groups
            .Select(g => g.GroupNumber)
            .OrderBy(g => g)       // в оригинале не было сортировки
            .ToList();

        foreach (var g in groups)
            Groups.Add(g);
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            MessageBox.Show("Введите ФИО студента", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(Group))
        {
            MessageBox.Show("Выберите группу", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!DateOnly.TryParseExact(
                BirthDateString, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            MessageBox.Show("Неверный формат даты рождения (дд.мм.гггг)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Базовая проверка на адекватность даты
        if (date.Year < 1900 || date > DateOnly.FromDateTime(DateTime.Today))
        {
            MessageBox.Show("Дата рождения не может быть в будущем или слишком далеко в прошлом", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid()) return;

        var date = DateOnly.ParseExact(BirthDateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        try
        {
            if (_id == null)
            {
                Db.Context.Students.Add(new Student
                {
                    FullName = FullName.Trim(),
                    GroupNumber = Group,
                    BirthDate = date
                });
            }
            else
            {
                var s = Db.Context.Students.First(x => x.StudentId == _id);

                s.FullName = FullName.Trim();
                s.GroupNumber = Group;
                s.BirthDate = date;
            }

            Db.Context.SaveChanges();
            window?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
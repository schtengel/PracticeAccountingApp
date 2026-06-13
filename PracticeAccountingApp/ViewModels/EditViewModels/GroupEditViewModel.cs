using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class GroupEditViewModel : BaseViewModel
{
    private readonly string? _originalGroupNumber;

    [ObservableProperty] private string groupNumber = "";
    [ObservableProperty] private string specialization = "";
    [ObservableProperty] private byte course = 1;

    public bool IsNewGroup => _originalGroupNumber == null;

    public ObservableCollection<byte> AvailableCourses { get; } = new();
    public ObservableCollection<Student> StudentsInGroup { get; } = new();

    public GroupEditViewModel(string? groupNumberToEdit = null)
    {
        _originalGroupNumber = groupNumberToEdit;

        for (byte i = 1; i <= 4 ; i++)
            AvailableCourses.Add(i);

        if (!string.IsNullOrEmpty(groupNumberToEdit))
            LoadExistingGroup(groupNumberToEdit);
    }

    private void LoadExistingGroup(string number)
    {
        var group = Db.Context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.GroupNumber == number);

        if (group == null) return;

        GroupNumber = group.GroupNumber;
        Specialization = group.Specialization;
        Course = group.Course;

        StudentsInGroup.Clear();
        foreach (var s in group.Students.OrderBy(s => s.FullName))
            StudentsInGroup.Add(s);
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(GroupNumber))
        {
            MessageBox.Show("Номер группы обязателен!", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (GroupNumber.Length > 8)
        {
            MessageBox.Show("Номер группы не может быть длиннее 8 символов", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(Specialization))
        {
            MessageBox.Show("Специальность обязательна!", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid()) return;

        try
        {
            if (_originalGroupNumber == null)
            {
                // Создание новой группы
                var normalized = GroupNumber.Trim().ToUpper();

                if (Db.Context.Groups.Any(g => g.GroupNumber == normalized))
                {
                    MessageBox.Show("Группа с таким номером уже существует!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Db.Context.Groups.Add(new Group
                {
                    GroupNumber = normalized,
                    Specialization = Specialization.Trim(),
                    Course = Course
                });
            }
            else
            {
                // Редактирование — GroupNumber НЕ меняем, так как это первичный ключ.
                // EF Core не поддерживает обновление PK через SaveChanges.
                // Поле заблокировано в XAML через IsNewGroup.
                var group = Db.Context.Groups
                    .FirstOrDefault(g => g.GroupNumber == _originalGroupNumber);

                if (group == null) return;

                group.Specialization = Specialization.Trim();
                group.Course = Course;
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
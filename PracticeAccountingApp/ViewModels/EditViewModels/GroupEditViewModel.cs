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

    public ObservableCollection<byte> AvailableCourses { get; } = new();
    public ObservableCollection<Student> StudentsInGroup { get; } = new();

    public GroupEditViewModel(string? groupNumberToEdit = null)
    {
        _originalGroupNumber = groupNumberToEdit;

        // Курсы от 1 до 6
        for (byte i = 1; i <= 6; i++)
            AvailableCourses.Add(i);

        if (!string.IsNullOrEmpty(groupNumberToEdit))
        {
            LoadExistingGroup(groupNumberToEdit);
        }
    }

    private void LoadExistingGroup(string groupNumber)
    {
        var group = Db.Context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.GroupNumber == groupNumber);

        if (group != null)
        {
            GroupNumber = group.GroupNumber;
            Specialization = group.Specialization;
            Course = group.Course;

            StudentsInGroup.Clear();
            foreach (var s in group.Students.OrderBy(s => s.FullName))
            {
                StudentsInGroup.Add(s);
            }
        }
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(GroupNumber) || string.IsNullOrWhiteSpace(Specialization))
        {
            MessageBox.Show("Номер группы и специальность обязательны!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (GroupNumber.Length > 8)
        {
            MessageBox.Show("Номер группы не может быть длиннее 8 символов", "Ошибка");
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
                // Создание
                if (Db.Context.Groups.Any(g => g.GroupNumber == GroupNumber.Trim().ToUpper()))
                {
                    MessageBox.Show("Группа с таким номером уже существует!", "Ошибка");
                    return;
                }

                var newGroup = new Group
                {
                    GroupNumber = GroupNumber.Trim().ToUpper(),
                    Specialization = Specialization.Trim(),
                    Course = Course
                };

                Db.Context.Groups.Add(newGroup);
            }
            else
            {
                // Редактирование
                var group = Db.Context.Groups.FirstOrDefault(g => g.GroupNumber == _originalGroupNumber);
                if (group == null) return;

                if (group.GroupNumber != GroupNumber.Trim().ToUpper() &&
                    Db.Context.Groups.Any(g => g.GroupNumber == GroupNumber.Trim().ToUpper()))
                {
                    MessageBox.Show("Группа с таким номером уже существует!", "Ошибка");
                    return;
                }

                group.GroupNumber = GroupNumber.Trim().ToUpper();
                group.Specialization = Specialization.Trim();
                group.Course = Course;
            }

            Db.Context.SaveChanges();
            window?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
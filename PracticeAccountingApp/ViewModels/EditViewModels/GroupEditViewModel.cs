using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PracticeAccountingApp.ViewModels;

public partial class GroupEditViewModel : BaseViewModel
{
    private readonly string? _originalGroupNumber;

    [ObservableProperty] private string groupNumber = "";
    [ObservableProperty] private string specialization = "";
    [ObservableProperty] private byte course = 1;

    public ObservableCollection<byte> AvailableCourses { get; } = new();

    public GroupEditViewModel(string? groupNumberToEdit = null)
    {
        _originalGroupNumber = groupNumberToEdit;

        for (byte i = 1; i <= 4; i++)
            AvailableCourses.Add(i);

        if (!string.IsNullOrEmpty(groupNumberToEdit))
        {
            LoadExistingGroup(groupNumberToEdit);
        }
    }

    private void LoadExistingGroup(string groupNumber)
    {
        var group = Db.Context.Groups.FirstOrDefault(g => g.GroupNumber == groupNumber);
        if (group != null)
        {
            GroupNumber = group.GroupNumber;
            Specialization = group.Specialization;
            Course = group.Course;
        }
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(GroupNumber) || string.IsNullOrWhiteSpace(Specialization))
            return false;

        if (GroupNumber.Length > 8)
            return false;

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid())
        {
            MessageBox.Show("Заполните номер группы и специальность корректно", "Ошибка",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_originalGroupNumber == null)
        {
            // Создание новой группы
            if (Db.Context.Groups.Any(g => g.GroupNumber == GroupNumber))
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
            if (group != null)
            {
                // Если номер группы изменился — проверяем уникальность
                if (group.GroupNumber != GroupNumber &&
                    Db.Context.Groups.Any(g => g.GroupNumber == GroupNumber))
                {
                    MessageBox.Show("Группа с таким номером уже существует!", "Ошибка");
                    return;
                }

                group.GroupNumber = GroupNumber.Trim().ToUpper();
                group.Specialization = Specialization.Trim();
                group.Course = Course;
            }
        }

        Db.Context.SaveChanges();
        window?.Close();
    }
}
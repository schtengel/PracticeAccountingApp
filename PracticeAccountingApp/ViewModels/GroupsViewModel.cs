using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class GroupsViewModel : BaseViewModel
{
    public ObservableCollection<GroupVm> Groups { get; } = new();

    public GroupsViewModel()
    {
        Load();
    }

    public void Load()
    {
        Groups.Clear();

        var data = Db.Context.Groups
            .OrderBy(g => g.GroupNumber)
            .Select(g => new GroupVm
            {
                GroupNumber = g.GroupNumber,
                Specialization = g.Specialization,
                Course = g.Course,
                StudentsCount = g.Students.Count
            })
            .ToList();

        foreach (var item in data)
            Groups.Add(item);
    }

    [RelayCommand]
    public void OpenAdd()
    {
        var win = new GroupEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Edit(GroupVm vm)
    {
        if (vm == null) return;

        var win = new GroupEditWindow(vm.GroupNumber);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    public void Delete(GroupVm vm)
    {
        if (vm == null) return;

        if (MessageBox.Show(
                $"Удалить группу {vm.GroupNumber} и всех её студентов?\n\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        try
        {
            var group = Db.Context.Groups
                .Include(g => g.Students)
                .Include(g => g.PracticeSheets)
                    .ThenInclude(ps => ps.StudentReports)
                .FirstOrDefault(g => g.GroupNumber == vm.GroupNumber);

            if (group == null) return;

            // Удаляем StudentReports → PracticeSheets → Students → Group
            // именно в таком порядке, иначе FK-ограничения выбросят исключение.
            foreach (var sheet in group.PracticeSheets)
                Db.Context.StudentReports.RemoveRange(sheet.StudentReports);

            Db.Context.PracticeSheets.RemoveRange(group.PracticeSheets);
            Db.Context.Students.RemoveRange(group.Students);
            Db.Context.Groups.Remove(group);

            Db.Context.SaveChanges();
            Groups.Remove(vm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // CanManageGroupsAndStudents и CanManagePractices унаследованы из BaseViewModel.
}

public class GroupVm
{
    public string GroupNumber { get; set; } = "";
    public string Specialization { get; set; } = "";
    public byte Course { get; set; }
    public int StudentsCount { get; set; }
}
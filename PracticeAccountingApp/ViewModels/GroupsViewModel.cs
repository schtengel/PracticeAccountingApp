using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;
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
            .Select(g => new GroupVm
            {
                GroupNumber = g.GroupNumber,
                Specialization = g.Specialization,
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
        if (MessageBox.Show($"Удалить группу {vm.GroupNumber} и всех её студентов?\n\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        var group = Db.Context.Groups
            .Include(g => g.Students)
            .FirstOrDefault(g => g.GroupNumber == vm.GroupNumber);

        if (group == null) return;

        Db.Context.Groups.Remove(group);
        Db.Context.SaveChanges();
        Load();
    }

    // Права доступа
    public bool CanManageGroupsAndStudents =>
        App.Current.MainWindow?.DataContext is MainViewModel mvm && mvm.CanManageGroupsAndStudents;

    public bool CanManagePractices =>
        App.Current.MainWindow?.DataContext is MainViewModel mvm && mvm.CanManagePractices;
}

public class GroupVm
{
    public string GroupNumber { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int StudentsCount { get; set; }
}
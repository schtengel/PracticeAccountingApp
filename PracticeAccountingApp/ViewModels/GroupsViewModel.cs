using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    private void Load()
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
    private void OpenAdd()
    {
        var win = new GroupEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    private void Delete(GroupVm vm)
    {
        if (MessageBox.Show($"Удалить группу {vm.GroupNumber}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        var group = Db.Context.Groups.FirstOrDefault(g => g.GroupNumber == vm.GroupNumber);
        if (group == null) return;

        Db.Context.Groups.Remove(group);
        Db.Context.SaveChanges();
        Load();
    }
}

public class GroupVm
{
    public string GroupNumber { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int StudentsCount { get; set; }
}
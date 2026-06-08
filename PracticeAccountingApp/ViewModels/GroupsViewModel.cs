using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;

namespace PracticeAccountingApp.ViewModels;

public partial class GroupsViewModel : BaseViewModel
{
    public ObservableCollection<GroupVm> Groups { get; } = [];

    public GroupsViewModel()
    {
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
}

public class GroupVm
{
    public string GroupNumber { get; set; } = "";
    public string Specialization { get; set; } = "";
    public int StudentsCount { get; set; }
}
using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticesViewModel : BaseViewModel
{
    public ObservableCollection<PracticeVm> Practices { get; } = [];

    public PracticesViewModel()
    {
        var data = Db.Context.PracticeSheets
            .Select(p => new PracticeVm
            {
                Id = p.PracticeSheetId,
                PracticeType = p.PracticeType.PracticeTypeName,
                Name = p.Module.ModuleName,
                Group = p.GroupNumber,
                Start = p.StartDate,
                End = p.EndDate
            })
            .ToList();

        foreach (var item in data)
            Practices.Add(item);
    }
}

public class PracticeVm
{
    public int Id { get; set; }
    public string PracticeType { get; set; } = "";
    public string Name { get; set; } = "";
    public string Group { get; set; } = "";
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}
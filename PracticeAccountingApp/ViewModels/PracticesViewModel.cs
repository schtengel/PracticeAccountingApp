using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using PracticeAccountingApp.Views.DialogWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticesViewModel : BaseViewModel
{
    [ObservableProperty]
    private string searchText = "";

    public ObservableCollection<PracticeVm> Practices { get; } = new();

    public PracticesViewModel()
    {
        Load();
    }

    partial void OnSearchTextChanged(string value)
    {
        Load();
    }

    private void Load()
    {
        Practices.Clear();

        var query = Db.Context.PracticeSheets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(p =>
                p.Module.ModuleName.Contains(SearchText) ||
                p.PracticeType.PracticeTypeName.Contains(SearchText) ||
                p.GroupNumber.Contains(SearchText));
        }

        var data = query
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

    [RelayCommand]
    private void OpenAdd()
    {
        var win = new PracticeEditWindow(null);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    private void Edit(PracticeVm vm)
    {
        var win = new PracticeEditWindow(vm.Id);
        win.ShowDialog();
        Load();
    }

    [RelayCommand]
    private void Delete(PracticeVm vm)
    {
        var entity = Db.Context.PracticeSheets
            .FirstOrDefault(x => x.PracticeSheetId == vm.Id);

        if (entity == null) return;

        Db.Context.PracticeSheets.Remove(entity);
        Db.Context.SaveChanges();

        Practices.Remove(vm);
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
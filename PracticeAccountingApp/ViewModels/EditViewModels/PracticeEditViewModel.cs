using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticeEditViewModel : BaseViewModel
{
    private readonly int? _id;

    [ObservableProperty] private string name = "";
    [ObservableProperty] private string group = "";
    [ObservableProperty] private string startString = "";
    [ObservableProperty] private string endString = "";

    public PracticeEditViewModel(int? id)
    {
        _id = id;

        if (id != null)
        {
            var p = Db.Context.PracticeSheets.First(x => x.PracticeSheetId == id);

            Name = p.Module.ModuleName;
            Group = p.GroupNumber;
            StartString = p.StartDate.ToString("dd.MM.yyyy");
            EndString = p.EndDate.ToString("dd.MM.yyyy");
        }
    }

    private bool IsValid()
    {
        return DateOnly.TryParseExact(StartString, "dd.MM.yyyy",
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out _) &&
               DateOnly.TryParseExact(EndString, "dd.MM.yyyy",
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid())
        {
            MessageBox.Show("Ошибка ввода дат");
            return;
        }

        var start = DateOnly.ParseExact(StartString, "dd.MM.yyyy");
        var end = DateOnly.ParseExact(EndString, "dd.MM.yyyy");

        if (_id == null)
        {
            Db.Context.PracticeSheets.Add(new PracticeSheet
            {
                GroupNumber = Group,
                StartDate = start,
                EndDate = end
            });
        }
        else
        {
            var p = Db.Context.PracticeSheets.First(x => x.PracticeSheetId == _id);

            p.GroupNumber = Group;
            p.StartDate = start;
            p.EndDate = end;
        }

        Db.Context.SaveChanges();

        window?.Close();
    }
}
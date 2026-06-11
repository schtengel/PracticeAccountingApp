using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticeEditViewModel : BaseViewModel
{
    private readonly int? _id;

    // выбранные объекты
    [ObservableProperty] private PracticeType selectedPracticeType;
    [ObservableProperty] private Module selectedModule;
    [ObservableProperty] private Teacher selectedTeacher;
    [ObservableProperty] private Group selectedGroup;

    // даты
    [ObservableProperty] private string startString = "";
    [ObservableProperty] private string endString = "";

    // коллекции
    public ObservableCollection<PracticeType> PracticeTypes { get; } = new();
    public ObservableCollection<Module> Modules { get; } = new();
    public ObservableCollection<Teacher> Teachers { get; } = new();
    public ObservableCollection<Group> Groups { get; } = new();

    public PracticeEditViewModel(int? id)
    {
        _id = id;

        foreach (var item in Db.Context.PracticeTypes)
            PracticeTypes.Add(item);

        foreach (var item in Db.Context.Modules)
            Modules.Add(item);

        foreach (var item in Db.Context.Teachers)
            Teachers.Add(item);

        foreach (var item in Db.Context.Groups)
            Groups.Add(item);

        if (id != null)
        {
            var p = Db.Context.PracticeSheets.First(x => x.PracticeSheetId == id);

            SelectedPracticeType = p.PracticeType;
            SelectedModule = p.Module;
            SelectedTeacher = p.Teacher;
            SelectedGroup = p.GroupNumberNavigation;

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
                PracticeTypeId = SelectedPracticeType.PracticeTypeId,
                ModuleId = SelectedModule.ModuleId,
                TeacherId = SelectedTeacher.TeacherId,
                GroupNumber = SelectedGroup.GroupNumber,
                StartDate = start,
                EndDate = end
            });
        }
        else
        {
            var p = Db.Context.PracticeSheets.First(x => x.PracticeSheetId == _id);

            p.PracticeTypeId = SelectedPracticeType.PracticeTypeId;
            p.ModuleId = SelectedModule.ModuleId;
            p.TeacherId = SelectedTeacher.TeacherId;
            p.GroupNumber = SelectedGroup.GroupNumber;
            p.StartDate = start;
            p.EndDate = end;
        }

        Db.Context.SaveChanges();
        window?.Close();
    }
}
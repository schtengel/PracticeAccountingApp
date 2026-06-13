using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class PracticeEditViewModel : BaseViewModel
{
    private readonly int? _id;

    [ObservableProperty] private PracticeType? selectedPracticeType;
    [ObservableProperty] private Module? selectedModule;
    [ObservableProperty] private Teacher? selectedTeacher;
    [ObservableProperty] private Group? selectedGroup;

    [ObservableProperty] private string startString = "";
    [ObservableProperty] private string endString = "";

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
            // Include обязателен: без него навигационные свойства == null,
            // и присвоение SelectedPracticeType = p.PracticeType бросало бы NRE.
            var p = Db.Context.PracticeSheets
                .Include(x => x.PracticeType)
                .Include(x => x.Module)
                .Include(x => x.Teacher)
                .Include(x => x.GroupNumberNavigation)
                .First(x => x.PracticeSheetId == id);

            // Ищем объекты из уже загруженных коллекций, чтобы ComboBox корректно
            // показывал выбранный элемент (сравнение по ссылке, а не по значению).
            SelectedPracticeType = PracticeTypes.FirstOrDefault(t => t.PracticeTypeId == p.PracticeTypeId);
            SelectedModule = Modules.FirstOrDefault(m => m.ModuleId == p.ModuleId);
            SelectedTeacher = Teachers.FirstOrDefault(t => t.TeacherId == p.TeacherId);
            SelectedGroup = Groups.FirstOrDefault(g => g.GroupNumber == p.GroupNumber);

            StartString = p.StartDate.ToString("dd.MM.yyyy");
            EndString = p.EndDate.ToString("dd.MM.yyyy");
        }
    }

    private bool IsValid()
    {
        if (SelectedPracticeType == null || SelectedModule == null ||
            SelectedTeacher == null || SelectedGroup == null)
        {
            MessageBox.Show("Заполните все поля", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!DateOnly.TryParseExact(StartString, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
        {
            MessageBox.Show("Неверный формат даты начала (дд.мм.гггг)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!DateOnly.TryParseExact(EndString, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
        {
            MessageBox.Show("Неверный формат даты окончания (дд.мм.гггг)", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (end < start)
        {
            MessageBox.Show("Дата окончания не может быть раньше даты начала", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid()) return;

        var start = DateOnly.ParseExact(StartString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        var end = DateOnly.ParseExact(EndString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        try
        {
            if (_id == null)
            {
                Db.Context.PracticeSheets.Add(new PracticeSheet
                {
                    PracticeTypeId = SelectedPracticeType!.PracticeTypeId,
                    ModuleId = SelectedModule!.ModuleId,
                    TeacherId = SelectedTeacher!.TeacherId,
                    GroupNumber = SelectedGroup!.GroupNumber,
                    StartDate = start,
                    EndDate = end
                });
            }
            else
            {
                var p = Db.Context.PracticeSheets.First(x => x.PracticeSheetId == _id);

                p.PracticeTypeId = SelectedPracticeType!.PracticeTypeId;
                p.ModuleId = SelectedModule!.ModuleId;
                p.TeacherId = SelectedTeacher!.TeacherId;
                p.GroupNumber = SelectedGroup!.GroupNumber;
                p.StartDate = start;
                p.EndDate = end;
            }

            Db.Context.SaveChanges();
            window?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class StudentEditViewModel : BaseViewModel
{
    private readonly int? _id;

    [ObservableProperty]
    private string fullName = "";

    [ObservableProperty]
    private string group = "";

    [ObservableProperty]
    private string birthDateString = "";

    public ObservableCollection<string> Groups { get; } = new();

    public StudentEditViewModel(int? id)
    {
        _id = id;

        LoadGroups();

        if (id != null)
        {
            var s = Db.Context.Students.First(x => x.StudentId == id);

            FullName = s.FullName;
            Group = s.GroupNumber;
            BirthDateString = s.BirthDate.ToString("dd.MM.yyyy");
        }
    }

    private void LoadGroups()
    {
        Groups.Clear();

        var groups = Db.Context.Groups
            .Select(g => g.GroupNumber)
            .ToList();

        foreach (var g in groups)
            Groups.Add(g);
    }

    private bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(FullName))
            return false;

        if (string.IsNullOrWhiteSpace(Group))
            return false;

        if (!DateOnly.TryParseExact(
                BirthDateString,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
            return false;

        return true;
    }

    [RelayCommand]
    private void Save(Window window)
    {
        if (!IsValid())
        {
            MessageBox.Show("Ошибка ввода данных");
            return;
        }

        var date = DateOnly.ParseExact(BirthDateString, "dd.MM.yyyy");

        if (_id == null)
        {
            Db.Context.Students.Add(new Student
            {
                FullName = FullName,
                GroupNumber = Group,
                BirthDate = date
            });
        }
        else
        {
            var s = Db.Context.Students.First(x => x.StudentId == _id);

            s.FullName = FullName;
            s.GroupNumber = Group;
            s.BirthDate = date;
        }

        Db.Context.SaveChanges();

        window?.Close();
    }
}
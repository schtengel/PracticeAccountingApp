using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System;
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
    private DateTime birthDate = DateTime.Now;

    public StudentEditViewModel(int? id)
    {
        _id = id;

        if (id != null)
        {
            var s = Db.Context.Students.First(x => x.StudentId == id);

            FullName = s.FullName;
            Group = s.GroupNumber;
            BirthDate = s.BirthDate.ToDateTime(TimeOnly.MinValue);
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (_id == null)
        {
            Db.Context.Students.Add(new Student
            {
                FullName = FullName,
                GroupNumber = Group,
                BirthDate = DateOnly.FromDateTime(BirthDate)
            });
        }
        else
        {
            var s = Db.Context.Students.First(x => x.StudentId == _id);

            s.FullName = FullName;
            s.GroupNumber = Group;
            s.BirthDate = DateOnly.FromDateTime(BirthDate);
        }

        Db.Context.SaveChanges();

        MessageBox.Show("Сохранено");
    }
}
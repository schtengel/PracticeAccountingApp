using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System;
using System.Linq;
using System.Windows;

namespace PracticeAccountingApp.ViewModels;

public partial class CreateUserViewModel : BaseViewModel
{
    [ObservableProperty]
    private string login = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private Role? selectedRole;

    [RelayCommand]
    private void CreateUser()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            return;

        var user = new User
        {
            Login = Login,
            PasswordHash = Password,
            RoleId = SelectedRole!.RoleId,
            RegistrationDate = DateTime.Now
        };

        Db.Context.Users.Add(user);
        Db.Context.SaveChanges();

        MessageBox.Show("Пользователь создан");
    }
}
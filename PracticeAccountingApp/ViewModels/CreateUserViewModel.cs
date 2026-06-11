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
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) || SelectedRole == null)
        {
            MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Db.Context.Users.Any(u => u.Login == Login))
        {
            MessageBox.Show("Пользователь с таким логином уже существует");
            return;
        }

        var user = new User
        {
            Login = Login.Trim(),
            PasswordHash = PasswordHelper.HashPassword(Password),
            RoleId = SelectedRole.RoleId,
            RegistrationDate = DateTime.Now
        };

        Db.Context.Users.Add(user);
        Db.Context.SaveChanges();

        MessageBox.Show("Пользователь создан успешно");
        // очистка полей
        Login = Password = "";
        SelectedRole = null;
    }
}
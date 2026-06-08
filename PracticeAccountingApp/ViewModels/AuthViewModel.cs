using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using PracticeAccountingApp.Views;
using System.Windows;
using System.Windows.Controls;

namespace PracticeAccountingApp.ViewModels;

public partial class AuthViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? login;

    [RelayCommand]
    private void LoginUser(PasswordBox passwordBox)
    {
        string password = passwordBox.Password;

        if (string.IsNullOrWhiteSpace(Login) ||
            string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(
                "Заполните логин и пароль",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        User? user = Db.Context.Users
            .FirstOrDefault(u =>
                u.Login == Login &&
                u.PasswordHash == password);

        if (user == null)
        {
            MessageBox.Show(
                "Неверный логин или пароль",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        MessageBox.Show(
            $"Добро пожаловать, {user.Login}",
            "Успешный вход",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        OpenMainWindow(user);
    }

    [RelayCommand]
    private void LoginAsGuest()
    {
        OpenMainWindow(null);
    }

    private static void OpenMainWindow(User? user)
    {
        MainWindow mainWindow = new(new MainViewModel(user));

        mainWindow.Show();

        Application.Current.Windows
            .OfType<AuthWindow>()
            .FirstOrDefault()
            ?.Close();
    }
}
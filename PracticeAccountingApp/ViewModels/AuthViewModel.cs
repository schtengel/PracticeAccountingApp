using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Models;
using PracticeAccountingApp.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PracticeAccountingApp.ViewModels;

public partial class AuthViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? login;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool isLoading = false;

    [RelayCommand]
    private async Task LoginUser(PasswordBox passwordBox)
    {
        string password = passwordBox.Password;
        ErrorMessage = "";
        IsLoading = true;

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Заполните логин и пароль";
            IsLoading = false;
            return;
        }

        // Имитация задержки сети + плавность
        await Task.Delay(400);

        User? user = Db.Context.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Login == Login);

        if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
        {
            ErrorMessage = "Неверный логин или пароль";
            IsLoading = false;
            return;
        }

        // Успешный вход
        ErrorMessage = "";
        await ShowSuccessAnimation();

        OpenMainWindow(user);
    }

    [RelayCommand]
    private void LoginAsGuest()
    {
        ErrorMessage = "";
        OpenMainWindow(null);
    }

    private async Task ShowSuccessAnimation()
    {
        // Здесь можно добавить логику анимации через событие или Messenger,
        // но для простоты просто небольшая задержка + сообщение
        // (анимацию сделаем в XAML)
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
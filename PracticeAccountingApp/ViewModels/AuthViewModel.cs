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
    [ObservableProperty] private string? login;
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private bool isLoading = false;

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

        await Task.Delay(400);

        User? user;
        await using (var ctx = Db.CreateContext())
        {
            user = await ctx.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == Login);
        }

        if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
        {
            ErrorMessage = "Неверный логин или пароль";
            IsLoading = false;
            return;
        }

        // Устанавливаем сессию ДО открытия MainWindow,
        // чтобы страницы при инициализации уже видели роль.
        AppSession.CurrentUser = user;

        ErrorMessage = "";
        OpenMainWindow(user);
    }

    [RelayCommand]
    private void LoginAsGuest()
    {
        AppSession.CurrentUser = null;
        ErrorMessage = "";
        OpenMainWindow(null);
    }

    private static void OpenMainWindow(User? user)
    {
        var mainWindow = new MainWindow(new MainViewModel(user));
        mainWindow.Show();

        Application.Current.Windows
            .OfType<AuthWindow>()
            .FirstOrDefault()
            ?.Close();
    }
}
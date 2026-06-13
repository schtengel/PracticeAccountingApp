using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PracticeAccountingApp.Models;
using System.Collections.ObjectModel;
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

    // Коллекция для ComboBox ролей — в оригинале отсутствовала,
    // поэтому выпадающий список был пустым.
    public ObservableCollection<Role> Roles { get; } = new();

    public CreateUserViewModel()
    {
        LoadRoles();
    }

    private void LoadRoles()
    {
        Roles.Clear();
        foreach (var role in Db.Context.Roles.OrderBy(r => r.RoleName))
            Roles.Add(role);
    }

    [RelayCommand]
    private void CreateUser()
    {
        if (string.IsNullOrWhiteSpace(Login))
        {
            MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedRole == null)
        {
            MessageBox.Show("Выберите роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Db.Context.Users.Any(u => u.Login == Login.Trim()))
        {
            MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var user = new User
            {
                Login = Login.Trim(),
                PasswordHash = PasswordHelper.HashPassword(Password),
                RoleId = SelectedRole.RoleId,
                RegistrationDate = DateTime.Now
            };

            Db.Context.Users.Add(user);
            Db.Context.SaveChanges();

            MessageBox.Show("Пользователь создан успешно", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            Login = "";
            Password = "";
            SelectedRole = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при создании пользователя: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
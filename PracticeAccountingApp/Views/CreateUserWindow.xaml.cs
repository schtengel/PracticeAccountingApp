using PracticeAccountingApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PracticeAccountingApp.Views;

public partial class CreateUserWindow : Window
{
    public CreateUserWindow()
    {
        InitializeComponent();
        DataContext = new CreateUserViewModel();
    }

    // PasswordBox не поддерживает двустороннюю привязку по соображениям безопасности,
    // поэтому синхронизируем вручную через событие PasswordChanged.
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is CreateUserViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}
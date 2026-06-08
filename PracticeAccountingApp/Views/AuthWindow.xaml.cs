using PracticeAccountingApp.ViewModels;

namespace PracticeAccountingApp.Views;

public partial class AuthWindow
{
    public AuthWindow()
    {
        InitializeComponent();
        DataContext = new AuthViewModel();
    }
}
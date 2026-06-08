using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;

namespace PracticeAccountingApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private bool isGuest;

    public bool IsAdmin => CurrentUser?.Role?.RoleName == "Администратор";

    public MainViewModel(User? user)
    {
        CurrentUser = user;
        IsGuest = user == null;

    }

    
}
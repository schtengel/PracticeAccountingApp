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
    public bool IsTeacher => CurrentUser?.Role?.RoleName == "Преподаватель";
    public bool IsUserOrGuest => !IsAdmin && !IsTeacher;

    public MainViewModel(User? user)
    {
        CurrentUser = user;
        IsGuest = user == null;
    }

    // Удобные свойства для привязки в XAML
    public bool CanManageGroupsAndStudents => IsAdmin;
    public bool CanManagePractices => IsAdmin || IsTeacher;
}
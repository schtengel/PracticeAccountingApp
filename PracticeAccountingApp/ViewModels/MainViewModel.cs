using CommunityToolkit.Mvvm.ComponentModel;
using PracticeAccountingApp.Models;

namespace PracticeAccountingApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAdmin))]
    [NotifyPropertyChangedFor(nameof(IsTeacher))]
    [NotifyPropertyChangedFor(nameof(IsUserOrGuest))]
    [NotifyPropertyChangedFor(nameof(CanManageGroupsAndStudents))]
    [NotifyPropertyChangedFor(nameof(CanManagePractices))]
    private User? currentUser;

    [ObservableProperty] private bool isGuest;

    public bool IsAdmin => AppSession.IsAdmin;
    public bool IsTeacher => AppSession.IsTeacher;
    public bool IsUserOrGuest => AppSession.IsGuest;

    // Перекрываем базовые свойства — здесь они те же, но явно видны в XAML MainWindow.
    public new bool CanManageGroupsAndStudents => AppSession.CanManageGroupsAndStudents;
    public new bool CanManagePractices => AppSession.CanManagePractices;

    public MainViewModel(User? user)
    {
        CurrentUser = user;
        IsGuest = user == null;
    }
}
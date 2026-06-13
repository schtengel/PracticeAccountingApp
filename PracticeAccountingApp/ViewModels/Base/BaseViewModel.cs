using CommunityToolkit.Mvvm.ComponentModel;

namespace PracticeAccountingApp.ViewModels;

public class BaseViewModel : ObservableValidator
{
    // Свойства читают из AppSession напрямую.
    // Страницы инициализируются ПОСЛЕ установки AppSession.CurrentUser,
    // поэтому значения корректны с первого обращения.
    public bool CanManageGroupsAndStudents => AppSession.CanManageGroupsAndStudents;
    public bool CanManagePractices => AppSession.CanManagePractices;
}
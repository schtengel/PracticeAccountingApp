using PracticeAccountingApp.Models;

namespace PracticeAccountingApp.ViewModels;

/// <summary>
/// Статический держатель сессии. Устанавливается при входе в систему
/// и читается из любой ViewModel без поиска по дереву окон.
/// </summary>
public static class AppSession
{
    public static User? CurrentUser { get; set; }

    public static bool IsAdmin
        => CurrentUser?.Role?.RoleName == "Администратор";

    public static bool IsTeacher
        => CurrentUser?.Role?.RoleName == "Преподаватель";

    public static bool IsGuest
        => CurrentUser == null;

    public static bool CanManageGroupsAndStudents
        => IsAdmin;

    public static bool CanManagePractices
        => IsAdmin || IsTeacher;

    public static void Clear() => CurrentUser = null;
}
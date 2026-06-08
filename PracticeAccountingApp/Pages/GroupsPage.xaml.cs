using PracticeAccountingApp.ViewModels;

namespace PracticeAccountingApp.Pages;

public partial class GroupsPage
{
    public GroupsPage()
    {
        InitializeComponent();
        DataContext = new GroupsViewModel();
    }
}
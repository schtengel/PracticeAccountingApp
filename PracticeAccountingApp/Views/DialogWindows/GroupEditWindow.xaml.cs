using PracticeAccountingApp.ViewModels;
using System.Windows;

namespace PracticeAccountingApp.Views.DialogWindows
{
    public partial class GroupEditWindow : Window
    {
        public GroupEditWindow(string? groupNumberToEdit = null)
        {
            InitializeComponent();
            DataContext = new GroupEditViewModel(groupNumberToEdit);
        }
    }
}
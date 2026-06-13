using System.Windows.Controls;
using PracticeAccountingApp.ViewModels;

namespace PracticeAccountingApp.Pages;

public partial class StudentsPage : Page
{
    public StudentsPage()
    {
        InitializeComponent();
        DataContext = new StudentsViewModel();
    }
}
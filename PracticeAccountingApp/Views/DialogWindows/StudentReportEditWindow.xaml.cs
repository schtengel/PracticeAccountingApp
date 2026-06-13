using PracticeAccountingApp.ViewModels;
using System.Windows;

namespace PracticeAccountingApp.Views.DialogWindows;

public partial class StudentReportEditWindow : Window
{
    public StudentReportEditWindow(int? reportId)
    {
        InitializeComponent();
        DataContext = new StudentReportEditViewModel(reportId);
    }
}
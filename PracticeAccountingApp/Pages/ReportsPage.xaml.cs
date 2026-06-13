using PracticeAccountingApp.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace PracticeAccountingApp.Pages;

public partial class ReportsPage : Page
{
    public ReportsPage()
    {
        InitializeComponent();
        DataContext = new ReportsViewModel();
    }

    private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow row && row.DataContext is ReportVm vm)
        {
            if (DataContext is ReportsViewModel model)
                model.Edit(vm);
        }
    }
}
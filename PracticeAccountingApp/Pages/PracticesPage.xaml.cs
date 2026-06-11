using PracticeAccountingApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PracticeAccountingApp.Pages
{
    public partial class PracticesPage : Page
    {
        public PracticesPage()
        {
            InitializeComponent();
            DataContext = new PracticesViewModel();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is PracticeVm practice)
            {
                var vm = (PracticesViewModel)DataContext;
                vm.Edit(practice);
            }
        }
    }
}
using PracticeAccountingApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PracticeAccountingApp.Pages
{
    public partial class GroupsPage : Page
    {
        public GroupsPage()
        {
            InitializeComponent();
            DataContext = new GroupsViewModel();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.DataContext is GroupVm group)
            {
                var vm = (GroupsViewModel)DataContext;
                vm.Edit(group);   // должно работать после того, как Edit сделан public
            }
        }
    }
}
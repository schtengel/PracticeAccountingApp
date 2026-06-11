using PracticeAccountingApp.ViewModels;
using System.Windows.Controls;

namespace PracticeAccountingApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PracticesPage.xaml
    /// </summary>
    public partial class PracticesPage : Page
    {
        public PracticesPage()
        {
            InitializeComponent();
            DataContext = new PracticesViewModel();
        }
    }
}

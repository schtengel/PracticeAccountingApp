using PracticeAccountingApp.ViewModels;
using System.Windows.Controls;


namespace PracticeAccountingApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Homepage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }
    }
}

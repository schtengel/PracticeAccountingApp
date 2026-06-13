using PracticeAccountingApp.ViewModels;
using System.Windows;

namespace PracticeAccountingApp.Views.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для StudentEditWindow.xaml
    /// </summary>
    public partial class StudentEditWindow : Window
    {
        public StudentEditWindow(int? id)
        {
            InitializeComponent();
            DataContext = new StudentEditViewModel(id);
        }
    }
}

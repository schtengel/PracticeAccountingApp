using PracticeAccountingApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

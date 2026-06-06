
using System;
using System.Windows;
using System.Windows.Controls;

namespace PracticeAccountingApp.Views
{
    public partial class MainWindow : Window
    {
        public string CurrentDate =>
            DateTime.Now.ToString("dd.MM.yyyy");

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            MainFrame.Navigate(
                new Uri("Pages/HomePage.xaml",
                UriKind.Relative));

            NavigationMenu.SelectedIndex = 0;
        }

        private void NavigationMenu_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (NavigationMenu.SelectedItem is ListBoxItem item)
            {
                string page = item.Tag.ToString();

                MainFrame.Navigate(
                    new Uri(page,
                    UriKind.Relative));
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            Close();
        }
    }
}


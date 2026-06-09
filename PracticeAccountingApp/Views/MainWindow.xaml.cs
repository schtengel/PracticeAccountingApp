using PracticeAccountingApp.Pages;
using PracticeAccountingApp.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PracticeAccountingApp.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        DataContext = _vm;

        // стартовая страница
        MainFrame.Navigate(new HomePage());

        // выделить Home по умолчанию
        NavigationMenu.SelectedIndex = 0;
    }

    private void NavigationMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (NavigationMenu.SelectedItem is not ListBoxItem item)
            return;

        string tag = item.Tag?.ToString() ?? "";

        Page page = tag switch
        {
            "Pages/HomePage.xaml" => new HomePage(),
            "Pages/StudentsPage.xaml" => new StudentsPage(),
            "Pages/GroupsPage.xaml" => new GroupsPage(),
            "Pages/PracticesPage.xaml" => new PracticesPage(),
            "Pages/ReportsPage.xaml" => new ReportsPage(),
            _ => new HomePage()
        };

        MainFrame.Navigate(page);
    }

    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        var auth = new AuthWindow();

        auth.Show();

        Close();
    }

    private void OpenCreateUser(object sender, RoutedEventArgs e)
    {
        new CreateUserWindow().ShowDialog();
    }
}
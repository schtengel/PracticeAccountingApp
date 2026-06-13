using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Data;
using System.Windows;

namespace PracticeAccountingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                if (!db.Database.CanConnect())
                {
                    db.Database.EnsureCreated();
                }
            }

            PasswordMigrationHelper.MigrateAllPasswords();

            base.OnStartup(e);
        }
    }

}

using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Data;
using System.Windows;

namespace PracticeAccountingApp
{
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

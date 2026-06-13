// Helpers/PasswordMigrationHelper.cs
using Microsoft.EntityFrameworkCore;
using PracticeAccountingApp.Data;
using System.Windows;

namespace PracticeAccountingApp;

public static class PasswordMigrationHelper
{
    public static void MigrateAllPasswords()
    {
        using var db = new AppDbContext();   // или Db.Context, если хочешь

        var users = db.Users.ToList();

        int migrated = 0;

        foreach (var user in users)
        {
            // Если пароль ещё не захэширован (короткий или не начинается с $2 — признак BCrypt)
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !user.PasswordHash.StartsWith("$2"))
            {
                string plainPassword = user.PasswordHash; // текущее значение — это и есть пароль
                user.PasswordHash = PasswordHelper.HashPassword(plainPassword);
                migrated++;
            }
        }

        if (migrated > 0)
        {
            db.SaveChanges();
            MessageBox.Show($"Успешно захэшировано {migrated} паролей!", "Миграция завершена");
        }
    }
}
using PracticeAccountingApp.Data;

namespace PracticeAccountingApp.ViewModels
{
    public static class Db
    {
        private static AppDbContext? _context;

        public static AppDbContext Context => _context ??= new AppDbContext();

        public static AppDbContext CreateContext() => new AppDbContext();
    }
}
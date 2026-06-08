using PracticeAccountingApp.Data;

namespace PracticeAccountingApp.ViewModels
{
    public static class Db
    {
        private static AppDbContext context = new();

        public static AppDbContext Context { get => context; set => context = value; }
    }
}

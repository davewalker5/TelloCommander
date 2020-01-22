using TelloCommander.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TelloCommander.Data.Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TelloCommanderDbContext context = new TelloCommanderDbContextFactory().CreateDbContext(null);
            context.Database.Migrate();
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TelloCommander.Data.InMemory
{
    public class TelloCommanderDbContextFactory : IDesignTimeDbContextFactory<TelloCommanderDbContext>
    {
        public TelloCommanderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TelloCommanderDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new TelloCommanderDbContext(optionsBuilder.Options);
        }
    }
}

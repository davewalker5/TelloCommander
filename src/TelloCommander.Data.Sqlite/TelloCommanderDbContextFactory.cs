using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TelloCommander.Data.Sqlite
{
    public class TelloCommanderDbContextFactory : IDesignTimeDbContextFactory<TelloCommanderDbContext>
    {
        public TelloCommanderDbContext CreateDbContext(string[] args)
        {
            // Construct a configuration object that contains the key/value pairs from the settings file
            // at the root of the main application
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                    .AddJsonFile("appsettings.json")
                                                    .Build();

            // Use the configuration object to read the connection string
            var optionsBuilder = new DbContextOptionsBuilder<TelloCommanderDbContext>();
            optionsBuilder.UseSqlite(configuration.GetConnectionString("TelloCommanderDb"));

            // Construct and return a database context
            return new TelloCommanderDbContext(optionsBuilder.Options);
        }
    }
}

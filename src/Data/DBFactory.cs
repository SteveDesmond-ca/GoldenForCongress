using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GoldenForCongress.Data
{
    public class DBFactory : IDesignTimeDbContextFactory<DB>
    {
        private readonly string _connectionString;

        public DBFactory() : this(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
        {
        }

        public DBFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DB");
        }

        public DB CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DB>();
            builder.UseSqlServer(_connectionString);
            return new DB(builder.Options);
        }
    }
}
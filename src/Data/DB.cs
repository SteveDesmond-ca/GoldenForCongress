using System.Threading.Tasks;
using GoldenForCongress.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoldenForCongress.Data
{
    public sealed class DB : IdentityDbContext
    {
        public DB(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Section> RouteSections { get; set; }
        public DbSet<Media> Media { get; set; }

        public async Task ClearLocationHistory()
        {
            await Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Locations");
            await SaveChangesAsync();
        }
    }
}

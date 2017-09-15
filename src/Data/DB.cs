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

        public DbSet<Section> RouteSections { get; set; }
        public DbSet<Media> Media { get; set; }
    }
}

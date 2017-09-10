using GoldenForCongress.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoldenForCongress.Data
{
    public class DB : IdentityDbContext
    {
        public DbSet<Route> Routes { get; set; }
    }
}

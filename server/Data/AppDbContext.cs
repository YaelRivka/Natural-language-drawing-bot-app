using Microsoft.EntityFrameworkCore;
using Server.Models; 

namespace server.Data
{
    public class AppDbContext : DbContext
    {
   
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Drawing> Drawings => Set<Drawing>();

    }
}


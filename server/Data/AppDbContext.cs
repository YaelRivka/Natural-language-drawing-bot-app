using Microsoft.EntityFrameworkCore;
using Server.Models; 

namespace server.Data
{
    public class AppDbContext : DbContext
    {
   
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Drawing> Drawings => Set<Drawing>();
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Drawing>()
                .HasOne<User>() 
                .WithMany()     
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade); // או Restrict אם לא רוצים מחיקה אוטומטית
        }
    }
}


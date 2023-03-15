using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Data
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options) { }
        public DbSet<NewsBoard> NewsBoards { get; set;}
        public DbSet<Client> Clients { get; set;}
        public DbSet<Subscription> Subscriptions { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<NewsBoard>().ToTable("NewsBoard");
            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.ClientId, s.NewsBoardId });
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
        }
    }
}

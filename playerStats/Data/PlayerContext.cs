using Microsoft.EntityFrameworkCore;
using playerStats.Models;

namespace playerStats.Data
{
    public class PlayerContext : DbContext
    {
        public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<Teams> Teams { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stats>()
                .HasOne(a => a.Player)
                .WithMany()
                .HasForeignKey(a => a.PlayerId);
            modelBuilder.Entity<Stats>()
                .HasOne(a => a.Game)
                .WithMany()
                .HasForeignKey(a => a.GameId);
            modelBuilder.Entity<Stats>()
                .HasOne(a => a.Team)
                .WithMany()
                .HasForeignKey(a => a.TeamId);
        }


    }
}

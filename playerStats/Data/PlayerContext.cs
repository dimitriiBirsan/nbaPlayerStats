using Microsoft.EntityFrameworkCore;
using playerStats.Models;

namespace playerStats.Data
{
    public class PlayerContext : DbContext
    {
        public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<User> Users { get; set; }

    }
}

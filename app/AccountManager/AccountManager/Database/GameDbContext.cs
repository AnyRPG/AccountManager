using Microsoft.EntityFrameworkCore;
using AccountManager.Models;

namespace AccountManager.Database
{
    public class GameDbContext : DbContext {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PlayerData> PlayerCharacters { get; set; }
    }
}

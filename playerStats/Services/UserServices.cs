using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Models;

namespace playerStats.Services
{
    public class UserService
    {
        
        private readonly PlayerContext _context;

        public UserService(PlayerContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

        }

        public async Task<User> CreateUserAsync(string email)
        {
            var user = new User { Email = email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task AddFavoritePlayerAsync(string email, Player player)
        {
            var user = await GetUserByEmailAsync(email);
            if (user !=null)
            {
                user.FavoritePlayers.Add(player);
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }

}

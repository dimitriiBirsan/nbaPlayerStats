using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace playerStats.Services
{
    public class UserService
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PlayerContext _context;

        public UserService(PlayerContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContextAccessor = httpContext;
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

        public async Task<IResult> AddFavoritePlayerAsync( Player player)

        {

            var email = _httpContextAccessor.HttpContext.Session.GetString("email");
            Debug.WriteLine(email);
            if(email == null)
            {
                return Results.BadRequest("user is not logged in");
            }
            var user = await GetUserByEmailAsync(email);
            if (user !=null)
            {
                var favoritePlayer = user.FavoritePlayers.FirstOrDefault(p => p.Id == player.Id);
                if (favoritePlayer != null)
                {
                    // Player is already a favorite, so remove it
                    user.FavoritePlayers.Remove(favoritePlayer);
                }
                else
                {
                    // Player is not a favorite, so add it
                    user.FavoritePlayers.Add(player);
                }
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            return Results.Ok();
        }

        public async Task<IResult> LoginAsync(string email )
        {

            if (string.IsNullOrEmpty(email))
            {
                Debug.WriteLine("no em");

                return Results.BadRequest("email is missing");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            Debug.WriteLine(email);
            _httpContextAccessor.HttpContext.Session.SetString("email", email);
            _httpContextAccessor.HttpContext.Session.CommitAsync().Wait();
            Debug.WriteLine(_httpContextAccessor.HttpContext.Session);

            if (existingUser != null)
            {
                return Results.Ok();
            }

            var newUser = new User { Email = email };
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return Results.Ok();
        }

        public async Task<IResult> CheckIfUserIsLoggedIn ()
        {

            var email = _httpContextAccessor.HttpContext.Session.GetString("email");
            Debug.WriteLine(_httpContextAccessor.HttpContext.Session);
            if(email == null)
            {
                Debug.WriteLine("no email");
                return Results.BadRequest("user is not logged in");
            }
            Debug.WriteLine(email);

            return Results.Ok();
        }
    }

}

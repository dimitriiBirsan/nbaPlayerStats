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

        public async Task<IResult> AddFavoritePlayerAsync(AddFavoritePlayer _favoritePlayer, HttpContext httpContext)

        {
            var email = _favoritePlayer.Email;
            if (email == null)
            {
              email = httpContext.Session.GetString("email");

            }

            Debug.WriteLine(email);
            if(email == null)
            {
                return Results.BadRequest("user is not logged in");
            }
            var user = await GetUserByEmailAsync(email);
            if (user !=null)
            {
                var favoritePlayer = user.FavoritePlayers?.FirstOrDefault(p => p.Id == _favoritePlayer.Id);
                var player = await _context.Players.FindAsync(_favoritePlayer.Id);
                if (favoritePlayer != null)
                {
                    // Player is already a favorite, so remove it
                    user.FavoritePlayers.Remove(player);
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

        public async Task<IResult> LoginAsync(string email, HttpContext httpContext )
        {

            if (string.IsNullOrEmpty(email))
            {
                Debug.WriteLine("no em");

                return Results.BadRequest("email is missing");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            httpContext.Session.SetString("email", email);

            if (existingUser != null)
            {
                return Results.Ok();
            }

            var newUser = new User { Email = email };
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return Results.Ok();
        }

        public async Task<IResult> CheckIfUserIsLoggedIn (HttpContext httpContext)
        {

            var email = httpContext.Session.GetString("email");
            Debug.WriteLine(httpContext.Session);
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

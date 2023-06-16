using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using playerStats.Data;
using playerStats.Models;
using System.Diagnostics;

namespace playerStats.Services
{
    public class PlayerService
    {
        private readonly HttpClient _httpClient;
        private readonly PlayerContext _playerContext;


        public PlayerService(HttpClient httpClient, PlayerContext context)
        {
            _httpClient = httpClient;
            _playerContext = context;
        }

        public async Task<List<Stats>> GetStatsAsync()
        {
            var playerStatistics = new List<Stats>();
            var currentPage = 1;
            var totalPages = 0;
            do
            {
                var response = await _httpClient.GetAsync($"https://www.balldontlie.io/api/v1/stats?seasons[]=2022&per_page=100&page={currentPage}");
                if(response.IsSuccessStatusCode)
                {
                    var pageData = await response.Content.ReadFromJsonAsync<StatsResponse>();
                    playerStatistics.AddRange(pageData.Data);

                    totalPages = pageData.Meta.Total_pages;
                    currentPage = pageData.Meta.Next_page;
                    if (currentPage <= totalPages)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            } while (currentPage < totalPages);
            return playerStatistics;
        }

        public async Task<IResult> GetAndSaveStatsAsync()
        {
            var playerStatistics = await GetStatsAsync();
            var teams = new List<Teams>();
            var players = new List<Player>();
            var games = new List<Games>();
            var stats = new List<Stats>();

            foreach (var record in playerStatistics)
            {
                var player = players.FirstOrDefault(p => p.Id == record.Player.Id);
                if (player == null)
                {
                    players.Add(record.Player);
                }

                var game = games.FirstOrDefault(g => g.Id == record.Game.Id);
                if (game == null)
                {
                    games.Add(record.Game);
                }

                var team = teams.FirstOrDefault(t => t.Id == record.Team.Id);
                if (team == null)
                {

                    teams.Add(record.Team);
                }


                stats.Add(record);
            }
            await InsertIfNotExistsAsync(_playerContext.Teams, teams);
            await InsertIfNotExistsAsync(_playerContext.Players, players);
            await InsertIfNotExistsAsync(_playerContext.Games, games);

            _playerContext .Stats.AddRange(stats);

            await _playerContext.SaveChangesAsync();

            return Results.Ok();
        }
        async Task InsertIfNotExistsAsync<T>(DbSet<T> dbSet, List<T> entities) where T : class
        {
            var existingEntities = await dbSet.ToListAsync();
            var newEntities = entities.Except(existingEntities);
            dbSet.AddRange(newEntities);
        }

        public async Task<List<Stats>> GetStatsForCurrentSeasonAsync()
        {
            var playerStatistics = new List<Stats>();
            var currentPage = 1;
            var totalPages = 0;

            do
            {
                var response = await _httpClient.GetAsync($"https://www.balldontlie.io/api/v1/stats?seasons[]=2022&per_page=100&page={currentPage}");

                if (response.IsSuccessStatusCode)
                {
                    var pageData = await response.Content.ReadFromJsonAsync<StatsResponse>();
                    playerStatistics.AddRange(pageData.Data);

                    totalPages = pageData.Meta.Total_pages;
                    currentPage = pageData.Meta.Next_page;
                    // Add a delay of 1 second before fetching the next page
                    if (currentPage <= totalPages)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }


                    foreach (var stat in pageData.Data)
                    {
                        var existingPlayer = await _playerContext.Players.FindAsync(stat.Player.Id);
                        var existingStat = await _playerContext.Stats.FindAsync(stat.Id);
                        var existingTeam = await _playerContext.Teams.FindAsync(stat.Team.Id);
                        var existingGame = await _playerContext.Games.FindAsync(stat.Game.Id);

                        if (existingPlayer == null)
                        {
                            _playerContext.Players.Add(stat.Player);
                        }
                        else
                        {
                            _playerContext.Players.Attach(existingPlayer);
                            stat.Player = existingPlayer;
                        }

                        if (existingTeam == null)
                        {
                            _playerContext.Teams.Add(stat.Team);
                        }
                        else
                        {
                            _playerContext.Teams.Attach(existingTeam);
                            stat.Team = existingTeam;
                        }

                        if (existingGame == null)
                        {
                            _playerContext.Games.Add(stat.Game);
                        }
                        else
                        {
                            _playerContext.Games.Attach(existingGame);
                            stat.Game = existingGame;
                        }
                        if(existingStat == null)
                        {
                            _playerContext.Stats.Add(stat);
                        }
                        await _playerContext.SaveChangesAsync();
                    }

 
                }
                else
                {
                    throw new Exception("Error fetching player data");
                };
            } while (currentPage < totalPages);

            return playerStatistics;
        }

        public async Task<List<PlayerAverage>> GetTrendiestPlayersAsync(string? userEmail, HttpContext httpContext )
        {
 
            if (userEmail == null)
            {
                userEmail = httpContext.Session.GetString("email");
            }
            var currentSeason = 2022;
            var statsForLastGames = 5;

            var favoritePlayerIds = new HashSet<int>();
            if (userEmail != null)
            {
                var user = await _playerContext.Users.Include(u => u.FavoritePlayers).FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user != null)
                {
                    favoritePlayerIds = new HashSet<int>(user.FavoritePlayers.Select(fp => fp.Id));
                }
            }
            var statsRecords = await _playerContext.Stats
                .Include(s => s.Game)
                .Include(s => s.Player)
                .Include (s => s.Team)
                .Where(s => s.Game.Season == currentSeason)
                .ToListAsync();

            var groupByPlayer = statsRecords.GroupBy(s => s.PlayerId);
            var playerAverage = new List<PlayerAverage>();

            foreach (var group in groupByPlayer)
            {
                var lastGames = group
                    .OrderByDescending(s => s.Game.Date)
                    .Take(statsForLastGames)
                    .ToList();

                var averagePoints = lastGames.Average(s => s.pts);

                var player = lastGames.Select(s => s.Player).First();
                var team = lastGames.Select(s => s.Team).First();

                playerAverage.Add(new PlayerAverage
                {
                    AveragePoints = averagePoints,
                    Player = player,
                    Teams = team,
                    IsFavorite = favoritePlayerIds.Contains(player.Id)
                });
            }

            var topPlayers = playerAverage
                .OrderByDescending(pa => pa.AveragePoints)
                .Take(10)
                .ToList();

            return topPlayers;
        }
    }
}


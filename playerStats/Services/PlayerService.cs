using playerStats.Data;
using playerStats.Models;
using System.Diagnostics;

namespace playerStats.Services
{
    public class PlayerService
    {
        private readonly HttpClient _httpClient;
        private readonly PlayerContext _playerContext;

        public PlayerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Player>?> GetAllPlayersAsync()
        {
            var allPlayers = new List<Player>();
            var currentPage = 1;
            var totalPages = 0;

            do
            {
                var response = await _httpClient.GetAsync($"https://www.balldontlie.io/api/v1/players?page={currentPage}&per_page=100");

                if(response.IsSuccessStatusCode)
                {
                    var pageData = await response.Content.ReadFromJsonAsync<PlayerResponse>();
                    allPlayers.AddRange(pageData.Data);

                    totalPages = pageData.Meta.Total_pages;
                    currentPage = pageData.Meta.Next_page;

                    // Add a delay of 1 second before fetching the next page
                    //if (currentPage <= totalPages)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    throw new Exception("Error ffetching player data");
                };
            } while (currentPage < totalPages);
            //if(response.IsSuccessStatusCode)
            //{
            //    var players = await response.Content.ReadFromJsonAsync<IEnumerable<Player>>();
            //    return players;
            //}
            Console.WriteLine(allPlayers);
            return allPlayers;
        }
        public async Task GetAndSaveAllPlayersAsync()
        {
            var players = await GetAllPlayersAsync();
            _playerContext.Players.AddRange(players);
            await _playerContext.SaveChangesAsync();
        }

        public async Task<List<Stats>> GetStatsForCurrentSeasonAsync()
        {
            var defaultLink = "https://www.balldontlie.io/api/v1/season_averages?season=2022";
            var playerStatistics = new List<Stats>();
            var players = await GetAllPlayersAsync();

            foreach (var item in players)
            {

                defaultLink = $"https://www.balldontlie.io/api/v1/season_averages?season=2022&player_ids[]={item.Id}";
                var response = await _httpClient.GetAsync(defaultLink);
                Debug.WriteLine(defaultLink);
                if(response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<StatsResponse>();
                    Console.WriteLine(stats);
                    if(stats.Data.Count > 0)
                    {
                        playerStatistics.AddRange(stats.Data);

                    }

                }
                // Add a delay of 1 second before fetching the next page
                //if (currentPage <= totalPages)
                //{
                await Task.Delay(TimeSpan.FromSeconds(1));
                //}


            }
            return playerStatistics;
        }

        public async Task<List<Stats>> GetTrendiestPlayersAsync()
        {
            var trendiestPlayers = new List <Stats>();
            var currentSeasonStats = await GetStatsForCurrentSeasonAsync();

            trendiestPlayers = currentSeasonStats.OrderByDescending(x => x.pts).Take(10).ToList();

            return trendiestPlayers;
        }
    }
}


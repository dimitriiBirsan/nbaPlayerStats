using HtmlAgilityPack;
using System.Net.Http;
using System.Diagnostics;
using playerStats.Models;

namespace playerStats.Scraper
{
    public class ScrapeRoster
    {
        public async Task<List<ScrapedPlayer>> ScrapeNBATeamsAsync()
        {
            var teams = new List<string>();
            const string TEAMS_URL = "https://www.espn.com/nba/team/roster/_/name/det/detroit-pistons";
            var httpClient = new HttpClient();

            var response = await httpClient.GetStringAsync(TEAMS_URL);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var teamsOptions = htmlDocument.DocumentNode.SelectNodes("//option[@class='dropdown__option']");

            foreach (var option in teamsOptions)
            {
                string teamUrl = option.GetAttributeValue("data-url", null);
                if(teamUrl != null & teamUrl.Contains("/_/name/" ) )
                {
                    teams.Add(teamUrl);
                }
            }
            var currentSeasonPlayers = new List<ScrapedPlayer>();
            foreach (var teamUrl in teams)
            {
                string fullTeamUrl = $"https://www.espn.com{teamUrl}";
                Debug.WriteLine(teamUrl);
                var teamRoaster = await ScrapeNBATeamRosterAsync(fullTeamUrl);
                currentSeasonPlayers.AddRange(teamRoaster);
            }

            return currentSeasonPlayers;

        }
        private async Task<List<ScrapedPlayer>> ScrapeNBATeamRosterAsync(string roster_url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(roster_url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml( response );

            var rosterTableBody = htmlDocument.DocumentNode.SelectSingleNode("//tbody[contains(@class, 'Table__TBODY')]").ChildNodes;
            var players = new List<string>();
            foreach (var node in rosterTableBody)
            {
                players.Add(node.InnerHtml);
            }
            var roaster = new List<ScrapedPlayer>();
            foreach (var player in players)
            {
                var playerInfo = new ScrapedPlayer();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml( player );
                var nameNode = htmlDoc.DocumentNode.SelectSingleNode(".//img");
                var name = nameNode.GetAttributeValue("title", "");
                playerInfo.Name = System.Net.WebUtility.HtmlDecode( name);

                var headshotNode = htmlDoc.DocumentNode.SelectSingleNode(".//img");
                var headshot = headshotNode.GetAttributeValue("alt", "");
                playerInfo.HeadShot = headshot;
                roaster.Add(playerInfo);
            }
            return roaster;
        }
    }
}

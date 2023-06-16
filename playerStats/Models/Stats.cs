namespace playerStats.Models
{
    public class Stats
    {
        public int Id { get; set; }
        public int? Ast { get; set; }
        public int? Blk { get; set; }
        public int? Dreb { get; set; }
        public float? Fg3_pct { get; set; }
        public float? Fg3a { get; set; }
        public float? Fg3m { get; set; }
        public float? Fg_pct { get; set; }
        public int? Fga { get; set; }
        public float? Fgm { get; set; }
        public float? Ft_pct { get; set; }
        public int? Fta { get; set; }
        public int? Ftm { get; set; }
        public string? Min { get; set; }
        public float? Oreb { get; set; }
        public int? Pf { get; set; }
        public int? pts { get; set; }
        public int? Reb { get; set; }
        public int? Stl { get; set; }
        public int? Turnover { get; set; }

        public int PlayerId { get; set; }
        public required Player Player { get; set; }

        public int GameId { get; set; }
        public required Games Game { get; set; }

        public int TeamId { get; set; }
        public required Teams Team { get; set; }

    }

    public class StatsResponse
    {
        public required List<Stats> Data { get; set; }
        public required Meta Meta { get; set; }
    }
}

namespace playerStats.Models
{
    public class Stats
    {
        public float Games_player { get; set; }
        public float Player_id { get; set; }
        public float Season { get; set; }
        public string Min { get; set; }
        public float Fgm { get; set; }
        public float Fga { get; set; }
        public float Fg3m { get; set; }
        public float Fg3a { get; set; }
        public float Ftm { get; set; }
        public float Fta { get; set; }
        public float Oreb { get; set; }
        public float Dreb { get; set; }
        public float Reb { get; set; }
        public float Ast { get; set; }
        public float Stl { get; set; }
        public float Blk { get; set; }
        public float Turnover { get; set; }
        public float Pf { get; set; }
        public float pts { get; set; }
        public float Fg_pct { get; set; }
        public float Fg3_pct { get; set; }
        public float Ft_pct { get; set; }

    }

    public class StatsResponse
    {
        public List<Stats> Data { get; set; }
    }
}

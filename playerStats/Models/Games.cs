namespace playerStats.Models
{
    public class Games
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int Home_team_id { get; set; }
        public int Home_team_score { get; set; }
        public int Season { get; set; }
        public int Visitor_team_id { get; set; }
        public int Visitor_team_score { get; set;}
    }
}

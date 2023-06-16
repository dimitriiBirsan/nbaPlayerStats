namespace playerStats.Models
{
    public class PlayerAverage
    { 
        public double? AveragePoints { get; set; }
        public Teams Teams { get; set; }
        public Player Player { get; set; }
        public bool IsFavorite { get; set; }
    }
}

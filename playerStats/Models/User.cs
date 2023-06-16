namespace playerStats.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public List<Player> FavoritePlayers { get; set; } = new List<Player>();


    }
}

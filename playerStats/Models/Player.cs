namespace playerStats.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public int? Height_feet { get; set; }
        public int? Height_inches { get; set; }
        public String Position { get; set; }
        public int Team_id { get; set; }
        public int? Weight_pounds { get; set; }


    }

    public class Meta
    {
        public int Total_pages { get; set; }
        public int Current_page { get; set; }

        public int Next_page { get; set; }
    }

    public class PlayerResponse
    {
        public List<Player> Data { get; set; }

        public Meta Meta { get; set; }
    }


}

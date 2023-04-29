namespace TubeTrackerAPI.Models
{
    public class Episode
    {
        public string air_date { get; set; }
        public int episode_number { get; set; }
        //public int id { get; set; }
        public string name { get; set; }
        //public string overview { get; set; }
        //public string production_code { get; set; }
        public int? runtime { get; set; }
        //public int season_number { get; set; }
        //public int show_id { get; set; }
        //public string still_path { get; set; }
        //public double vote_average { get; set; }
        //public int vote_count { get; set; }
        //public List<Crew> crew { get; set; }
        //public List<GuestStar> guest_stars { get; set; }

        public bool watched { get; set; }

        public bool favorite { get; set; }
    }
    /*public class ExternalSeasonsList
    {
        public List<ExternalSeasonDetails> external_season_details { get; set; }
    }*/

    public class ExternalSeasonDetails
    {
        //public string _id { get; set; }
        //public string air_date { get; set; }
        public List<Episode> episodes { get; set; }
        //public string name { get; set; }
        //public string overview { get; set; }
        //public int id { get; set; }
        //public string poster_path { get; set; }
        public int season_number { get; set; }
    }


}

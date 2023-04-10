namespace TubeTrackerAPI.Models
{
    public class SerieCast
    {
        public bool adult { get; set; }
        public int gender { get; set; }
        public int id { get; set; }
        public string known_for_department { get; set; }
        public string name { get; set; }
        public string original_name { get; set; }
        public double popularity { get; set; }
        public string profile_path { get; set; }
        public string character { get; set; }
        public string credit_id { get; set; }
        public int order { get; set; }
    }

    public class SerieCreatedBy
    {
        //public int id { get; set; }
        //public string credit_id { get; set; }
        public string name { get; set; }
        //public int gender { get; set; }
        //public string profile_path { get; set; }
    }

    public class SerieCredits
    {
        public List<SerieCast> cast { get; set; }
    }

    public class SerieGenre
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class SerieResult
    {
        public string iso_639_1 { get; set; }
        public string iso_3166_1 { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public DateTime published_at { get; set; }
        public string site { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public bool official { get; set; }
        public string id { get; set; }
    }

    public class ExternalSerieDetails
    {
        //public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public List<SerieCreatedBy> created_by { get; set; }
        //public List<int> episode_run_time { get; set; }
        public string first_air_date { get; set; }
        public List<SerieGenre> genres { get; set; }
        //public string homepage { get; set; }
        public int id { get; set; }
        //public bool in_production { get; set; }
        //public List<string> languages { get; set; }
        //public string last_air_date { get; set; }
        //public LastEpisodeToAir last_episode_to_air { get; set; }
        public string name { get; set; }
        //public object next_episode_to_air { get; set; }
        //public List<Network> networks { get; set; }
        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        //public List<string> origin_country { get; set; }
        //public string original_language { get; set; }
        //public string original_name { get; set; }
        public string overview { get; set; }
        //public double popularity { get; set; }
        public string poster_path { get; set; }
        //public List<ProductionCompany> production_companies { get; set; }
        //public List<ProductionCountry> production_countries { get; set; }
        //public List<SerieSeason> seasons { get; set; }
        //public List<SpokenLanguage> spoken_languages { get; set; }
        //public string status { get; set; }
        //public string tagline { get; set; }
        //public string type { get; set; }
        //public double vote_average { get; set; }
        //public int vote_count { get; set; }
        public SerieVideos videos { get; set; }
        public SerieCredits credits { get; set; }
    }

    public class SerieVideos
    {
        public List<SerieResult> results { get; set; }
    }


}

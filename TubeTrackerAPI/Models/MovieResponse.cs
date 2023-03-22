namespace TubeTrackAPI.Models
{
    public class MovieResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}

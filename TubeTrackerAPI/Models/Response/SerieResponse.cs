namespace TubeTrackerAPI.Models.Response
{
    public class SerieResponse
    {
        public int Page { get; set; }
        public List<ExternalSerie> Results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}

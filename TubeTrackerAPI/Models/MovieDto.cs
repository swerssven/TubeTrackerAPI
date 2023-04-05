namespace TubeTrackerAPI.Models
{
    public class MovieDto
    {
        public int MovieId { get; set; }

        public int MovieApiId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Actors { get; set; }

        public string Directors { get; set; }

        public string Genres { get; set; }

        public DateTime? PremiereDate { get; set; }

        public string Trailer { get; set; }

        public string Poster { get; set; }

        public string Backdrop { get; set; }

        public int? Duration { get; set; }
    }
}

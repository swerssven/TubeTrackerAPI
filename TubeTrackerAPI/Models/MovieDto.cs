namespace TubeTrackerAPI.Models
{
    public class MovieDto
    {
        public int MovieId { get; set; }

        public int MovieApiId { get; set; }

        public string TitleEn { get; set; }

        public string TitleEs { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionEs { get; set; }

        public string Actors { get; set; }

        public string Directors { get; set; }

        public string GenresEn { get; set; }

        public string GenresEs { get; set; }

        public DateTime? PremiereDate { get; set; }

        public string Poster { get; set; }

        public string Backdrop { get; set; }

        public int? Duration { get; set; }

        public string TrailerEs { get; set; }

        public string TrailerEn { get; set; }

        public bool? watched { get; set; }

        public bool? favorite { get; set; }

        public DateTime DateAddedFavorite { get; set; }
    }
}

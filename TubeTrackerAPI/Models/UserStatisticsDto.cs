namespace TubeTrackerAPI.Models
{
    public class UserStatisticsDto
    {
        public int TotalHoursSeries { get; set; }

        public int WatchedEpisodes { get; set; }

        public int TotalHoursMovies { get; set; }

        public int WatchedMovies { get; set; }

        public int Posts { get; set; }

        public int LikesPosts { get; set; }

        public int PostComments { get; set; }

        public int Friends { get; set; }
    }
}

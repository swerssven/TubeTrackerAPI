namespace TubeTrackerAPI.Models.Request
{
    public class CreateMovieReviewListRequest
    {
        public int UserId { get; set; }

        public int MovieApiId { get; set; }

        public string Content { get; set; }
    }
}

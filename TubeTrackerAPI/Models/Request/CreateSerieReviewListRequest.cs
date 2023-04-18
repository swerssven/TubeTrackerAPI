namespace TubeTrackerAPI.Models.Request
{
    public class CreateSerieReviewListRequest
    {
        public int UserId { get; set; }

        public int SerieApiId { get; set; }

        public string Content { get; set; }
    }
}

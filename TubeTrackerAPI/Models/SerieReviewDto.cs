namespace TubeTrackerAPI.Models
{
    public class SerieReviewItemDto
    {
        public int SerieReviewId { get; set; }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string UserImage { get; set; }

        public int SerieId { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public int Rating { get; set; }
    }

    public class SerieReviewDto
    {
        public int numReviews { get; set; }

        public List<SerieReviewItemDto> reviews { get; set; }
    }
}

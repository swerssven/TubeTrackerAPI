namespace TubeTrackerAPI.Models
{
    public class SerieReviewDto
    {
        public int SerieReviewId { get; set; }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string UserImage { get; set; }

        public int SerieId { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

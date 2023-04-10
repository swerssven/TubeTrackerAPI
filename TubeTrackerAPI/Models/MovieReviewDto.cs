namespace TubeTrackerAPI.Models
{
    public class MovieReviewDto
    {
        public int MovieReviewId { get; set; }

        public int UserId { get; set; }

        public string UserNickname { get; set; }

        public string UserImage { get; set; }

        public int MovieId { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

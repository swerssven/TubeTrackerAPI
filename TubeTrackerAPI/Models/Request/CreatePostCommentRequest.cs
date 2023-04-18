namespace TubeTrackerAPI.Models.Request
{
    public class CreatePostCommentRequest
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

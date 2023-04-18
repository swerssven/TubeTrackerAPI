namespace TubeTrackerAPI.Models
{
    public class PostCommentDto
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public string UserImage { get; set; }

        public string UserNickname { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

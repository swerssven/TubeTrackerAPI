using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class PostDto
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public string UserImage { get; set; }

        public string UserNickname { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public int LikesCount { get; set; }

        public bool LikedByUser { get; set; }

        public virtual ICollection<PostComment> PostComments { get; set; }
    }
}

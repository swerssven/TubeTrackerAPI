using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models.Request
{
    public class CreatePostRequest
    {
        public int PostId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

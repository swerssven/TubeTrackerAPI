using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class NewsDto
    {
        public int NewsId { get; set; }

        public int UserId { get; set; }

        public string creatorNickname { get; set; }

        public string userImage { get; set; }

        public string ContentEn { get; set; }

        public string ContentEs { get; set; }

        public DateTime CreationDate { get; set; }

    }
}

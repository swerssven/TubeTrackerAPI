using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class NewsDto
    {
        public int NewsId { get; set; }

        public string creatorNickname { get; set; }

        public string TitleEn { get; set; }

        public string TitleEs { get; set; }

        public string ContentEn { get; set; }

        public string ContentEs { get; set; }

        public DateTime CreationDate { get; set; }

    }
}

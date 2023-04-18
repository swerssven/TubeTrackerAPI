namespace TubeTrackerAPI.Models.Request
{
    public class CreateNewsRequest
    {
        public int UserId { get; set; }

        public string TitleEn { get; set; }

        public string TitleEs { get; set; }

        public string ContentEn { get; set; }

        public string ContentEs { get; set; }
    }
}

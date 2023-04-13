namespace TubeTrackerAPI.Models
{
    public class FriendDto
    {
        public int UserId { get; set; }

        public string FriendNickname { get; set; }

        public string FriendImage { get; set; }

        public int FriendshipStatus { get; set; }
    }
}

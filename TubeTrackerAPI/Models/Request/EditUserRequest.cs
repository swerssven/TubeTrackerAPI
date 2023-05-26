namespace TubeTrackerAPI.Models.Request
{
    public class EditUserRequest
    {
        public int UserId { get; set; }

        public string Nickname { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Language { get; set; }

        public string Image { get; set; }
    }
}

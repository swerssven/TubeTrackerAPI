namespace TubeTrackerAPI.Models
{
    public class UserToken
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}

namespace TubeTrackerAPI.Models
{
    public class usersGridDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }
    }
}

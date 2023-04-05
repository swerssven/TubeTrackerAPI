using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public string Language { get; set; }

        public string Image { get; set; }

        public int RolId { get; set; }
    }
}


using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models
{
    public class MessageDto
    {
        public string ReceiverImage { get; set; }

        public string ReceiverName { get; set; }

        public IEnumerable<Message> MessagesList { get; set; } = new List<Message>();
    }
}

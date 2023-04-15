namespace TubeTrackerAPI.Models.Request
{
    public class CreateMessageRequest
    {
        public int SenderUserId { get; set; }

        public int ReceiverUserId { get; set; }

        public string Content { get; set; }
    }
}

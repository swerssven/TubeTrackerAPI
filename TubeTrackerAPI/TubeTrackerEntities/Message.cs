using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Message
{
    public int SenderUserId { get; set; }

    public int ReceiverUserId { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public bool IsRead { get; set; }

    public virtual User ReceiverUser { get; set; }

    public virtual User SenderUser { get; set; }
}

using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class PostComment
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual Post Post { get; set; }

    public virtual User User { get; set; }
}

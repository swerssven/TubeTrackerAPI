using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class News
{
    public int NewsId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual User User { get; set; }
}

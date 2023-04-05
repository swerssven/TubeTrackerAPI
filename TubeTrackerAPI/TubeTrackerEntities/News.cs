using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class News
{
    public int NewsId { get; set; }

    public int UserId { get; set; }

    public string TitleEn { get; set; }

    public string TitleEs { get; set; }

    public string ContentEn { get; set; }

    public string ContentEs { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual User User { get; set; }
}

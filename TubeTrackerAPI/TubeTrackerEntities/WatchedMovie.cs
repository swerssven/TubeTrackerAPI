using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class WatchedMovie
{
    public int MovieId { get; set; }

    public int UserId { get; set; }

    public DateTime DateWatched { get; set; }

    public virtual Movie Movie { get; set; }

    public virtual User User { get; set; }
}

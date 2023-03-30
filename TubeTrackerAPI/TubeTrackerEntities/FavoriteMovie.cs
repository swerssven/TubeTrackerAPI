using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class FavoriteMovie
{
    public int MovieId { get; set; }

    public int UserId { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual Movie Movie { get; set; }

    public virtual User User { get; set; }
}

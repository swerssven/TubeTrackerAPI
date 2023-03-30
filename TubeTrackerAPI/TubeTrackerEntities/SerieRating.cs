using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class SerieRating
{
    public int SerieId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public virtual Series Serie { get; set; }

    public virtual User User { get; set; }
}

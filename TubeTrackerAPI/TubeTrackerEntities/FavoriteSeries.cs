using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class FavoriteSeries
{
    public int SerieId { get; set; }

    public int UserId { get; set; }

    public DateTime DateAdded { get; set; }

    public virtual Series Serie { get; set; }

    public virtual User User { get; set; }
}

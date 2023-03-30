using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class WatchedSeriesSeasonsEpisode
{
    public int SerieId { get; set; }

    public int SeasonsEpisodesId { get; set; }

    public int UserId { get; set; }

    public DateTime DateWatched { get; set; }

    public virtual SeasonsEpisode Serie { get; set; }

    public virtual Series SerieNavigation { get; set; }

    public virtual User User { get; set; }
}

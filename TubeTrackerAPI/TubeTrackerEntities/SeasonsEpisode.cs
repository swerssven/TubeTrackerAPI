using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class SeasonsEpisode
{
    public int SeasonsEpisodesId { get; set; }

    public int SerieId { get; set; }

    public int NumSeason { get; set; }

    public int NumEpisode { get; set; }

    public int? EpisodeDuration { get; set; }

    public string TitleEpisode { get; set; }

    public DateTime? PremiereDate { get; set; }

    public virtual Series Serie { get; set; }

    public virtual ICollection<WatchedSeriesSeasonsEpisode> WatchedSeriesSeasonsEpisodes { get; } = new List<WatchedSeriesSeasonsEpisode>();
}

using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Series
{
    public int SerieId { get; set; }

    public int SerieApiId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Actors { get; set; }

    public string Creators { get; set; }

    public string Genres { get; set; }

    public DateTime? PremiereDate { get; set; }

    public string Trailer { get; set; }

    public string Poster { get; set; }

    public string Backdrop { get; set; }

    public virtual ICollection<FavoriteSeries> FavoriteSeries { get; } = new List<FavoriteSeries>();

    public virtual ICollection<SeasonsEpisode> SeasonsEpisodes { get; } = new List<SeasonsEpisode>();

    public virtual ICollection<SerieRating> SerieRatings { get; } = new List<SerieRating>();

    public virtual ICollection<SerieReview> SerieReviews { get; } = new List<SerieReview>();

    public virtual ICollection<WatchedSeriesSeasonsEpisode> WatchedSeriesSeasonsEpisodes { get; } = new List<WatchedSeriesSeasonsEpisode>();
}

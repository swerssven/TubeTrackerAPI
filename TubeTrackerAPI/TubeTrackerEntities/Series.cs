using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Series
{
    public int SerieId { get; set; }

    public int SerieApiId { get; set; }

    public string TitleEn { get; set; }

    public string TitleEs { get; set; }

    public string DescriptionEn { get; set; }

    public string DescriptionEs { get; set; }

    public string Actors { get; set; }

    public string Creators { get; set; }

    public string GenresEn { get; set; }

    public string GenresEs { get; set; }

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

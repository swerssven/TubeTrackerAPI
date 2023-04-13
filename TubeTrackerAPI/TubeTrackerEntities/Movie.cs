using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Movie
{
    public int MovieId { get; set; }

    public int MovieApiId { get; set; }

    public string TitleEn { get; set; }

    public string TitleEs { get; set; }

    public string DescriptionEn { get; set; }

    public string DescriptionEs { get; set; }

    public string Actors { get; set; }

    public string Directors { get; set; }

    public string GenresEn { get; set; }

    public string GenresEs { get; set; }

    public DateTime? PremiereDate { get; set; }

    public string Poster { get; set; }

    public string Backdrop { get; set; }

    public int? Duration { get; set; }

    public string TrailerEs { get; set; }

    public string TrailerEn { get; set; }

    public virtual ICollection<FavoriteMovie> FavoriteMovies { get; } = new List<FavoriteMovie>();

    public virtual ICollection<MovieRating> MovieRatings { get; } = new List<MovieRating>();

    public virtual ICollection<MovieReview> MovieReviews { get; } = new List<MovieReview>();

    public virtual ICollection<WatchedMovie> WatchedMovies { get; } = new List<WatchedMovie>();
}

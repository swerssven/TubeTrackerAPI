using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Nickname { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Language { get; set; }

    public string Image { get; set; }

    public int RolId { get; set; }

    public DateTime RegistryDate { get; set; }

    public virtual ICollection<FavoriteMovie> FavoriteMovies { get; } = new List<FavoriteMovie>();

    public virtual ICollection<FavoriteSeries> FavoriteSeries { get; } = new List<FavoriteSeries>();

    public virtual ICollection<MovieRating> MovieRatings { get; } = new List<MovieRating>();

    public virtual ICollection<MovieReview> MovieReviews { get; } = new List<MovieReview>();

    public virtual ICollection<News> News { get; } = new List<News>();

    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual ICollection<SerieRating> SerieRatings { get; } = new List<SerieRating>();

    public virtual ICollection<SerieReview> SerieReviews { get; } = new List<SerieReview>();

    public virtual ICollection<WatchedMovie> WatchedMovies { get; } = new List<WatchedMovie>();

    public virtual ICollection<WatchedSeriesSeasonsEpisode> WatchedSeriesSeasonsEpisodes { get; } = new List<WatchedSeriesSeasonsEpisode>();

    public virtual ICollection<Post> PostsNavigation { get; } = new List<Post>();
}

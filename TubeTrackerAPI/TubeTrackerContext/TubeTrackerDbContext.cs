using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.TubeTrackerContext;

public partial class TubeTrackerDbContext : DbContext
{
    public TubeTrackerDbContext()
    {
    }

    public TubeTrackerDbContext(DbContextOptions<TubeTrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FavoriteMovie> FavoriteMovies { get; set; }

    public virtual DbSet<FavoriteSeries> FavoriteSeries { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieRating> MovieRatings { get; set; }

    public virtual DbSet<MovieReview> MovieReviews { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<SeasonsEpisode> SeasonsEpisodes { get; set; }

    public virtual DbSet<SerieRating> SerieRatings { get; set; }

    public virtual DbSet<SerieReview> SerieReviews { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WatchedMovie> WatchedMovies { get; set; }

    public virtual DbSet<WatchedSeriesSeasonsEpisode> WatchedSeriesSeasonsEpisodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoriteMovie>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.UserId }).HasName("PK_FavoriteMovies_MovieId_UserId");

            entity.Property(e => e.DateAdded).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.FavoriteMovies)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteMovies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FavoriteSeries>(entity =>
        {
            entity.HasKey(e => new { e.SerieId, e.UserId }).HasName("PK_FavoriteSeries_SerieId_UserId");

            entity.Property(e => e.DateAdded).HasColumnType("datetime");

            entity.HasOne(d => d.Serie).WithMany(p => p.FavoriteSeries)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteSeries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.UserId, e.FriendUserId }, "UK_Friends_UserId_FriendUserId").IsUnique();

            entity.HasOne(d => d.FriendUser).WithMany()
                .HasForeignKey(d => d.FriendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.ReceiverUser).WithMany()
                .HasForeignKey(d => d.ReceiverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SenderUser).WithMany()
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK_Movies_MovieId");

            entity.HasIndex(e => e.MovieApiId, "UK_Movies_MovieApiId").IsUnique();

            entity.Property(e => e.Actors)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Backdrop)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Directors)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Genres)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Poster)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PremiereDate).HasColumnType("date");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Trailer)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MovieRating>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.UserId }).HasName("PK_MovieRatings_MovieId_UserId");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieRatings)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.MovieRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MovieReview>(entity =>
        {
            entity.HasKey(e => e.MovieReviewId).HasName("PK_MovieReviews_MovieReviewId");

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieReviews)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.MovieReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK_News_NewsId");

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK_Posts_PostId");

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Users).WithMany(p => p.PostsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "PostsLike",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("PostId", "UserId").HasName("PK_PostsLikes_PostId_UserId");
                        j.ToTable("PostsLikes");
                    });
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany()
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SeasonsEpisode>(entity =>
        {
            entity.HasKey(e => e.SeasonsEpisodesId).HasName("PK_SeasonsEpisodes_SeasonsEpisodesId");

            entity.HasIndex(e => new { e.SerieId, e.NumSeason, e.NumEpisode }, "UK_SeasonsEpisodes_SerieId_NumSeason_NumEpisode").IsUnique();

            entity.Property(e => e.PremiereDate).HasColumnType("date");
            entity.Property(e => e.TitleEpisode)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Serie).WithMany(p => p.SeasonsEpisodes)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SerieRating>(entity =>
        {
            entity.HasKey(e => new { e.SerieId, e.UserId }).HasName("PK_SerieRatings_SerieId_UserId");

            entity.HasOne(d => d.Serie).WithMany(p => p.SerieRatings)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.SerieRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SerieReview>(entity =>
        {
            entity.HasKey(e => e.SerieReviewId).HasName("PK_SerieReviews_SerieReviewId");

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Serie).WithMany(p => p.SerieReviews)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.SerieReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.HasKey(e => e.SerieId).HasName("PK_Series_SerieId");

            entity.HasIndex(e => e.SerieApiId, "UK_Series_SerieApiId").IsUnique();

            entity.Property(e => e.Actors)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Backdrop)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Creators)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Genres)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Poster)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PremiereDate).HasColumnType("date");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Trailer)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users_UserId");

            entity.HasIndex(e => e.Email, "UK_Users_Email").IsUnique();

            entity.HasIndex(e => e.Nickname, "UK_Users_Nickname").IsUnique();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Image).IsUnicode(false);
            entity.Property(e => e.Language)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nickname)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RegistryDate).HasColumnType("datetime");
            entity.Property(e => e.RolId).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<WatchedMovie>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.UserId }).HasName("PK_WatchedMovies_MovieId_UserId");

            entity.Property(e => e.DateWatched).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.WatchedMovies)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.WatchedMovies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WatchedSeriesSeasonsEpisode>(entity =>
        {
            entity.HasKey(e => new { e.SerieId, e.SeasonsEpisodesId, e.UserId }).HasName("PK_WatchedSeriesSeasonsEpisodes_SerieId_UserId");

            entity.Property(e => e.DateWatched).HasColumnType("datetime");

            entity.HasOne(d => d.Serie).WithMany(p => p.WatchedSeriesSeasonsEpisodes)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WatchedSeriesSeasonsEpisodes_SeasonsEpisodes_SeasonsEpisodesId");

            entity.HasOne(d => d.SerieNavigation).WithMany(p => p.WatchedSeriesSeasonsEpisodes)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.WatchedSeriesSeasonsEpisodes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

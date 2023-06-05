using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text.RegularExpressions;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    public class MovieRepository
    {
        private const string URL = "https://api.themoviedb.org/3";
        private const string apiKey = "7d22105ae1b958ce88fe42db67a97318";
        private readonly TubeTrackerDbContext _dbContext;

        public MovieRepository(TubeTrackerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        // Gets a list of movie with the necesary information from external API to make movie-cards
        public async Task<string> GetMovieSearchList(string filter, int page, string language)
        {
            string apiURL = $"/search/movie?api_key={apiKey}&language={language}&query={filter}&page={page}&include_adult=false";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URL + apiURL))
                {
                    string apiResponse = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }

                    return apiResponse;
                }
            }
        }

        public async Task<MovieResponse> GetMoviePopularList()
        {
            MovieResponse movieResponse = new MovieResponse();
            movieResponse.Results = new List<ExternalMovie>();
            var query = @"
                SELECT TOP 20 m.*, 
                    ISNULL(w.WatchCount, 0) AS WatchCount, 
                    ISNULL(f.FavoriteCount, 0) AS FavoriteCount
                FROM Movies AS m
                LEFT JOIN (
                    SELECT MovieId, COUNT(*) AS WatchCount
                    FROM WatchedMovies
                    GROUP BY MovieId
                ) AS w ON m.MovieId = w.MovieId
                LEFT JOIN (
                    SELECT MovieId, COUNT(*) AS FavoriteCount
                    FROM FavoriteMovies
                    GROUP BY MovieId
                ) AS f ON m.MovieId = f.MovieId
                ORDER BY (ISNULL(w.WatchCount, 0) + ISNULL(f.FavoriteCount, 0)) DESC";

            var popularMovies = await _dbContext.Movies
                .FromSqlRaw(query)
                .ToListAsync();

            foreach (var m in popularMovies)
            {
                DateTime date = DateTime.ParseExact(m.PremiereDate.ToString(), "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("yyyy-MM-dd");
                ExternalMovie externalMovie = new ExternalMovie()
                {
                    id = m.MovieApiId,
                    title = (m.TitleEn != null ? m.TitleEn : m.TitleEs),
                    release_date = formattedDate,
                    poster_path = m.Poster,
                    backdrop_path = m.Backdrop
                };
                movieResponse.Results.Add(externalMovie);
            }

            return movieResponse;
        }

        public async Task<MovieResponse> GetMovieTopRatedList()
        {
            MovieResponse movieResponse = new MovieResponse();
            movieResponse.Results = new List<ExternalMovie>();
            var query = @"
                SELECT TOP 20 m.*, 
                    ISNULL(r.RatingCount, 0) AS RatingCount
                FROM Movies AS m
                LEFT JOIN (
                    SELECT MovieId, COUNT(*) AS RatingCount
                    FROM MovieRatings
                    GROUP BY MovieId
                ) AS r ON m.MovieId = r.MovieId
                ORDER BY ISNULL(r.RatingCount, 0) DESC";

            var topRatedMovies = await _dbContext.Movies
                .FromSqlRaw(query)
                .ToListAsync();

            foreach (var m in topRatedMovies)
            {
                DateTime date = DateTime.ParseExact(m.PremiereDate.ToString(), "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("yyyy-MM-dd");
                ExternalMovie externalMovie = new ExternalMovie()
                {
                    id = m.MovieApiId,
                    title = (m.TitleEn != null ? m.TitleEn : m.TitleEs),
                    release_date = formattedDate,
                    poster_path = m.Poster,
                    backdrop_path = m.Backdrop
                };
                movieResponse.Results.Add(externalMovie);
            }

            return movieResponse;
        }

        // Create new movie in data base
        public async Task<MovieDto> CreateMovie(Movie movie, int userId)
        {
            var MovieQuery = await _dbContext.Movies.Include(m => m.MovieReviews).Where(m => m.MovieApiId == movie.MovieApiId).FirstOrDefaultAsync();
            //MovieQuery = (from m in _dbContext.Movies where m.MovieApiId == movie.MovieApiId select m).FirstOrDefault();

            if (MovieQuery == null)
            {
                _dbContext.Movies.Add(movie);
                await _dbContext.SaveChangesAsync();
                MovieQuery = movie;
            }
            else if (MovieQuery.TitleEn == null && movie.TitleEn != null)
            {
                MovieQuery.TitleEn = movie.TitleEn;
                MovieQuery.DescriptionEn = movie.DescriptionEn;
                MovieQuery.GenresEn = movie.GenresEn;
                MovieQuery.TrailerEn = movie.TrailerEn;
                _dbContext.Movies.Update(MovieQuery);
                await _dbContext.SaveChangesAsync();
            }
            else if (MovieQuery.TitleEs == null && movie.TitleEs != null)
            {
                MovieQuery.TitleEs = movie.TitleEs;
                MovieQuery.DescriptionEs = movie.DescriptionEs;
                MovieQuery.GenresEs = movie.GenresEs;
                MovieQuery.TrailerEs = movie.TrailerEs;
                _dbContext.Movies.Update(MovieQuery);
                await _dbContext.SaveChangesAsync();
            }

            MovieDto movieDto = new MovieDto();
            movieDto.MovieId = MovieQuery.MovieId;
            movieDto.MovieApiId = MovieQuery.MovieApiId;
            movieDto.TitleEn = MovieQuery.TitleEn;
            movieDto.TitleEs = MovieQuery.TitleEs;
            movieDto.DescriptionEn = MovieQuery.DescriptionEn;
            movieDto.DescriptionEs = MovieQuery.DescriptionEs;
            movieDto.Actors = MovieQuery.Actors;
            movieDto.Directors = MovieQuery.Directors;
            movieDto.GenresEn = MovieQuery.GenresEn;
            movieDto.GenresEs = MovieQuery.GenresEs;
            movieDto.PremiereDate = MovieQuery.PremiereDate;
            movieDto.Poster = MovieQuery.Poster;
            movieDto.Backdrop = MovieQuery.Backdrop;
            movieDto.Duration = MovieQuery.Duration;
            movieDto.TrailerEs = MovieQuery.TrailerEs;
            movieDto.TrailerEn = MovieQuery.TrailerEn;
            await this.checkWatchedAndFavoriteMovie(movieDto, userId);

            return movieDto;
        }

        // Gets a movie from external API
        public async Task<string> GetMovieExternal(int id, string language)
        {
            string apiURL = $"/movie/{id}?api_key={apiKey}&language={language}&append_to_response=videos,credits";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URL + apiURL))
                {
                    string apiResponse = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }

                    return apiResponse;
                }
            }
        }

        public async Task<MovieReviewDto> GetMovieReviews(int movieApiId)
        {
            var movieQuery = await _dbContext.Movies.Include(m => m.MovieReviews).Where(m => m.MovieApiId == movieApiId).FirstOrDefaultAsync();

            MovieReviewDto movieReviewDto = new MovieReviewDto();

            movieReviewDto.reviews = await _dbContext.MovieReviews.Where(m => m.MovieId == movieQuery.MovieId)
                .Select(m => new MovieReviewItemDto()
                {
                    MovieReviewId = m.MovieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate,
                    Rating = _dbContext.MovieRatings.Where(x => x.UserId == m.UserId && x.MovieId == m.MovieId).Select(x => x.Rating).FirstOrDefault()
                }).ToListAsync();

            movieReviewDto.numReviews = await _dbContext.MovieReviews.CountAsync(m => m.MovieId == movieQuery.MovieId);

            return movieReviewDto;
        }

        public async Task<bool> DeleteMovieReview(int movieReviewId)
        {
            var queryMovieReview = await _dbContext.MovieReviews.Where(m => m.MovieReviewId == movieReviewId).FirstOrDefaultAsync();
            if (queryMovieReview != null)
            {
                _dbContext.Remove(queryMovieReview);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            };
            return false; ;
        }

        public async Task<MovieReviewDto> CreateMovieReviewList(CreateMovieReviewListRequest request)
        {
            var movieQuery = await _dbContext.Movies.Include(m => m.MovieReviews).Where(m => m.MovieApiId == request.MovieApiId).FirstOrDefaultAsync();
            var reviewQuery = movieQuery.MovieReviews.Where(m => m.UserId == request.UserId).FirstOrDefault();

            MovieReview movieReview = new MovieReview();
            movieReview.Content = request.Content;
            movieReview.UserId = request.UserId;
            movieReview.CreationDate = DateTime.UtcNow;

            if (reviewQuery == null && request.Content != null)
            {
                movieReview.MovieId = movieQuery.MovieId;
                _dbContext.MovieReviews.Add(movieReview);
                await _dbContext.SaveChangesAsync();
            }
            else if (reviewQuery != null)
            {
                reviewQuery.Content = movieReview.Content;
                reviewQuery.CreationDate = movieReview.CreationDate;
                _dbContext.MovieReviews.Update(reviewQuery);
                await _dbContext.SaveChangesAsync();
            }

            MovieReviewDto movieReviewDto = new MovieReviewDto();

            movieReviewDto.reviews = await _dbContext.MovieReviews.Where(m => m.MovieId == movieQuery.MovieId)
                .Select(m => new MovieReviewItemDto()
                {
                    MovieReviewId = m.MovieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            movieReviewDto.numReviews = await _dbContext.MovieReviews.CountAsync(m => m.MovieId == movieQuery.MovieId);

            return movieReviewDto;
        }

        public async Task<RatingsDto> SetMovieRating(MovieRating movieRating)
        {
            var ratingQuery = _dbContext.MovieRatings.Where(r => r.MovieId == movieRating.MovieId && r.UserId == movieRating.UserId).FirstOrDefault();

            if (ratingQuery == null)
            {
                _dbContext.MovieRatings.Add(movieRating);
                await _dbContext.SaveChangesAsync();
                ratingQuery = movieRating;
            }
            else
            {
                ratingQuery.Rating = movieRating.Rating;
                _dbContext.MovieRatings.Update(ratingQuery);
                await _dbContext.SaveChangesAsync();
            }

            return await GetMovieRatings(movieRating.UserId, ratingQuery.Movie.MovieApiId);
        }

        public async Task<RatingsDto> GetMovieRatings(int userId, int movieApiId)
        {
            RatingsDto ratingsDto = new RatingsDto();
            int movieId = await getMovieDbId(movieApiId);
            var RatingQuery = await _dbContext.MovieRatings.Where(r =>  r.MovieId == movieId).ToListAsync();

            if (!RatingQuery.IsNullOrEmpty())
            {
                ratingsDto.AverageRating = RatingQuery.Average(r => r.Rating);
                var tmp = RatingQuery.FirstOrDefault(r => r.UserId == userId);

                if (tmp != null)
                {
                    ratingsDto.UserRating = tmp.Rating;
                }
            }

            return ratingsDto;
        }

        public async Task<bool> setMovieWatched(int movieId, int userId, bool watched)
        {
            bool result = watched;

            if (watched)
            {
                WatchedMovie watchedMovie = new WatchedMovie();
                watchedMovie.MovieId = movieId;
                watchedMovie.UserId = userId;
                watchedMovie.DateWatched = DateTime.Now;

                _dbContext.WatchedMovies.Add(watchedMovie);
                await _dbContext.SaveChangesAsync();
                result = true;
            }
            else if (!watched)
            {
                var watchedMovieQuery = await _dbContext.WatchedMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();
                _dbContext.WatchedMovies.Remove(watchedMovieQuery);
                await _dbContext.SaveChangesAsync();
                result = false;
            }

            return result;
        }

        // Mark/unmark movie as favorite.
        public async Task<bool> setMovieFavorite(int movieId, int userId, bool isFavorite)
        {
            bool result = isFavorite;

            if (isFavorite)
            {
                FavoriteMovie favoriteMovie = new FavoriteMovie();
                favoriteMovie.MovieId = movieId;
                favoriteMovie.UserId = userId;
                favoriteMovie.DateAdded = DateTime.Now;

                _dbContext.FavoriteMovies.Add(favoriteMovie);
                await _dbContext.SaveChangesAsync();
                result = true;
            }
            else if (!isFavorite)
            {
                var favoriteMovieQuery = await _dbContext.FavoriteMovies.Where(f => f.MovieId == movieId && f.UserId == userId).FirstOrDefaultAsync();
                _dbContext.FavoriteMovies.Remove(favoriteMovieQuery);
                await _dbContext.SaveChangesAsync();
                result = false;
            }

            return result;
        }

        // Get user's favorites movies.
        public async Task<IEnumerable<ExternalMovie>> getMovieFavoritesList(int userId, string language)
        {
            List<int> movieIds = await _dbContext.FavoriteMovies.Where(f => f.UserId == userId).Select(m => m.MovieId).ToListAsync();
            IEnumerable<ExternalMovie> favoriteMovieList = new List<ExternalMovie>();
            
            if(language == "en-EN")
            {
                favoriteMovieList = await _dbContext.Movies.Where(m => movieIds.Contains(m.MovieId))
                    .Select(m => new ExternalMovie()
                    {
                        id = m.MovieApiId,
                        title = m.TitleEn,
                        release_date = m.PremiereDate.ToString(),
                        poster_path = m.Poster,
                        backdrop_path = m.Backdrop
                    }).ToListAsync();
            }
            else if (language == "es-ES")
            {
                favoriteMovieList = await _dbContext.Movies.Where(m => movieIds.Contains(m.MovieId))
                    .Select(m => new ExternalMovie()
                    {
                        id = m.MovieApiId,
                        title = m.TitleEs,
                        release_date = m.PremiereDate.ToString(),
                        poster_path = m.Poster,
                        backdrop_path = m.Backdrop
                    }).ToListAsync();
            }

            foreach (var movie in favoriteMovieList)
            {
                await checkWatchedAndFavoriteMovie(movie, userId);
                movie.DateAddedFavorite = await _dbContext.FavoriteMovies.Where(f => f.UserId == userId && f.MovieId == movie.id).
                    Select(s => s.DateAdded).FirstOrDefaultAsync();
            }

            return favoriteMovieList;
        }

        // Get's database id with the external api id.
        public async Task<int> getMovieDbId(int movieApiId)
        {
            var movie = await _dbContext.Movies.Where(m => m.MovieApiId == movieApiId).FirstOrDefaultAsync();

            if (movie == null)
            {
                return 0;
            }

            return movie.MovieId;
        }

        // Check if a movies list is watched and favorited by specific user.
        public async Task<MovieResponse> checkWatchedAndFavoriteMoviesFromList(MovieResponse movieResponse, int userId)
        {
            foreach (var movie in movieResponse.Results)
            {
                var movieId = await this.getMovieDbId(movie.id);

                var watchedQuery = await _dbContext.WatchedMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

                if (watchedQuery != null)
                {
                    movie.watched = true;
                }
                else
                {
                    movie.watched = false;
                }

                var favoriteQuery = await _dbContext.FavoriteMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

                if (favoriteQuery != null)
                {
                    movie.favorite = true;
                }
                else
                {
                    movie.favorite = false;
                }
            }

            return movieResponse;
        }

        // Check if movie is watched and favorited by specific user with movieDto.
        public async Task<MovieDto> checkWatchedAndFavoriteMovie(MovieDto movie, int userId)
        {
            var movieId = await this.getMovieDbId(movie.MovieApiId);

            var watchedQuery = await _dbContext.WatchedMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

            if (watchedQuery != null)
            {
                movie.watched = true;
            }
            else
            {
                movie.watched = false;
            }

            var favoriteQuery = await _dbContext.FavoriteMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

            if (favoriteQuery != null)
            {
                movie.favorite = true;
            }
            else
            {
                movie.favorite = false;
            }

            return movie;
        }

        // Check if movie is watched and favorited by specific user with external movie.
        public async Task<ExternalMovie> checkWatchedAndFavoriteMovie(ExternalMovie movie, int userId)
        {
            var movieId = await this.getMovieDbId(movie.id);

            var watchedQuery = await _dbContext.WatchedMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

            if (watchedQuery != null)
            {
                movie.watched = true;
            }
            else
            {
                movie.watched = false;
            }

            var favoriteQuery = await _dbContext.FavoriteMovies.Where(w => w.MovieId == movieId && w.UserId == userId).FirstOrDefaultAsync();

            if (favoriteQuery != null)
            {
                movie.favorite = true;
            }
            else
            {
                movie.favorite = false;
            }

            return movie;
        }

        // Get user's last 12 watched movies.
        public async Task<IEnumerable<ExternalMovie>> getLastWatchedMoviesList(int userId, string language)
        {
            List<int> movieIds = await _dbContext.WatchedMovies.OrderByDescending(m => m.DateWatched).Take(6).Where(f => f.UserId == userId).Select(m => m.MovieId).ToListAsync();
            IEnumerable<ExternalMovie> watchedMovieList = new List<ExternalMovie>();

            if (language == "en-EN")
            {
                watchedMovieList = await _dbContext.Movies.Where(m => movieIds.Contains(m.MovieId))
                    .Select(m => new ExternalMovie()
                    {
                        id = m.MovieApiId,
                        title = m.TitleEn,
                        release_date = m.PremiereDate.ToString(),
                        poster_path = m.Poster,
                        backdrop_path = m.Backdrop
                    }).ToListAsync();
            }
            else if (language == "es-ES")
            {
                watchedMovieList = await _dbContext.Movies.Where(m => movieIds.Contains(m.MovieId))
                    .Select(m => new ExternalMovie()
                    {
                        id = m.MovieApiId,
                        title = m.TitleEs,
                        release_date = m.PremiereDate.ToString(),
                        poster_path = m.Poster,
                        backdrop_path = m.Backdrop
                    }).ToListAsync();
            }

            foreach (var movie in watchedMovieList)
            {
                await checkWatchedAndFavoriteMovie(movie, userId);
                movie.DateAddedFavorite = await _dbContext.FavoriteMovies.Where(f => f.UserId == userId && f.MovieId == movie.id).
                    Select(s => s.DateAdded).FirstOrDefaultAsync();
            }

            return watchedMovieList;
        }
    }
}

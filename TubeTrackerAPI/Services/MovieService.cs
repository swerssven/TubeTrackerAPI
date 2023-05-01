using Newtonsoft.Json;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    public class MovieService
    {
        private readonly TubeTrackerDbContext _dbContext;

        private readonly MovieRepository _repository;

        public MovieService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new MovieRepository(dbContext);
        }

        public async Task<MovieResponse> GetMovieSearchList(string filter, int page, string language, int userId)
        {
            string resultStr = await new MovieRepository(this._dbContext).GetMovieSearchList(filter, page, language);

            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(resultStr);

            movieResponse = await _repository.checkWatchedAndFavoriteMoviesFromList(movieResponse, userId);

            return movieResponse;  
        }

        public async Task<MovieResponse> GetMoviePopularList(string language, int userId)
        {
            string resultStr = await new MovieRepository(this._dbContext).GetMoviePopularList(language);

            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(resultStr);

            movieResponse = await _repository.checkWatchedAndFavoriteMoviesFromList(movieResponse, userId);

            return movieResponse;
        }

        public async Task<MovieResponse> GetMovieTopRatedList(string language, int userId)
        {
            string resultStr = await _repository.GetMovieTopRatedList(language);

            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(resultStr);

            movieResponse = await _repository.checkWatchedAndFavoriteMoviesFromList(movieResponse, userId);

            return movieResponse;
        }

        public async Task<MovieDto> CreateMovie(int id, string language, int userId)
        {
            string resultStr = await _repository.GetMovieExternal(id, language);

            ExternalMovieDetails externalMovieDetailsResponse = JsonConvert.DeserializeObject<ExternalMovieDetails>(resultStr);

            string trailer = externalMovieDetailsResponse.videos.results.Where(t => t.type == "Trailer").FirstOrDefault()?.key;

            List<MovieCast> cast = externalMovieDetailsResponse.credits.cast.Where(c => c.known_for_department == "Acting").ToList();
            string actors = null;
            if(cast != null)
            {
                actors = string.Join(", ", cast.Select(c => c.name).Take(20).ToList());
            }

            List<MovieCrew> crew = externalMovieDetailsResponse.credits.crew.Where(c => c.job == "Director").ToList();
            string directors = null;
            if (crew != null)
            {
                directors = string.Join(", ", crew.Select(c => c.name).Take(20).ToList());
            }

            List<MovieGenre> genreList = externalMovieDetailsResponse.genres.ToList();
            string genres = null;
            if (genreList != null)
            {
                genres = string.Join(", ", genreList.Select(g => g.name).Take(20).ToList());
            }

            Movie movie = new Movie();

            movie.MovieApiId = externalMovieDetailsResponse.id;
            movie.Actors = actors;
            movie.Directors = directors;
            movie.PremiereDate = DateTimeOffset.Parse(externalMovieDetailsResponse.release_date).UtcDateTime;
            movie.Poster = externalMovieDetailsResponse.poster_path;
            movie.Backdrop = externalMovieDetailsResponse.backdrop_path;
            movie.Duration = externalMovieDetailsResponse.runtime;

            if (language == "en-EN")
            {
                movie.TitleEn = externalMovieDetailsResponse.title;
                movie.DescriptionEn = externalMovieDetailsResponse.overview;
                movie.GenresEn = genres;
                movie.TrailerEn = trailer;
            } 
            else if (language == "es-ES")
            {
                movie.TitleEs = externalMovieDetailsResponse.title;
                movie.DescriptionEs = externalMovieDetailsResponse.overview;
                movie.GenresEs = genres;
                movie.TrailerEs = trailer;
            }

            return await _repository.CreateMovie(movie, userId); 
        }

        public async Task<MovieReviewDto> GetMovieReviews(int movieApiId)
        {
            MovieReviewDto movieReviewResponse = await _repository.GetMovieReviews(movieApiId);

            return movieReviewResponse;
        }
        
        public async Task<MovieReviewDto> CreateMovieReviewList(CreateMovieReviewListRequest request)
        {
            MovieRepository movieRepository = new MovieRepository(_dbContext);

            MovieReviewDto movieReviewResponse = await movieRepository.CreateMovieReviewList(request);

            return movieReviewResponse;
        }

        public async Task<RatingsDto> SetMovieRating(int movieApiId, int userId, int rating)
        {
            MovieRating movieRating = new MovieRating();

            movieRating.MovieId = await _repository.getMovieDbId(movieApiId);
            movieRating.UserId = userId;
            movieRating.Rating = rating;

            return await _repository.SetMovieRating(movieRating);
        }

        public async Task<RatingsDto> GetMovieRatings(int userId, int movieApiId)
        {
            return await _repository.GetMovieRatings(userId, movieApiId);
        }

        public async Task<bool> setMovieWatched(int movieApiId, int userId, string language, bool watched)
        {
            var movieId = await _repository.getMovieDbId(movieApiId);

            if (movieId == 0) // Check if movie in database, if not, it will be created.
            {
                await this.CreateMovie(movieApiId, language, userId);
                movieId = await _repository.getMovieDbId(movieApiId);
            }
            return await _repository.setMovieWatched(movieId, userId, watched);
        }

        public async Task<bool> setMovieFavorite(int movieApiId, int userId, string language, bool favorite)
        {
            var movieId = await _repository.getMovieDbId(movieApiId);

            if (movieId == 0) // Check if movie in database, if not, it will be created.
            {
                await this.CreateMovie(movieApiId, language, userId);
                movieId = await _repository.getMovieDbId(movieApiId);
            }
            return await _repository.setMovieFavorite(movieId, userId, favorite);
        }

        public async Task<IEnumerable<MovieDto>> getMovieFavoritesList(int userId)
        {
            return await _repository.getMovieFavoritesList(userId);
        }
    }
}

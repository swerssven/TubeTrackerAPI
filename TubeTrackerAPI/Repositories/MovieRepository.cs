using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models;
using Azure.Core;
using Microsoft.IdentityModel.Tokens;

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
            //List<ExternalMovie> movieList = new List<ExternalMovie>();

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

        // Create new movie in data base
        public async Task<Movie> CreateMovie(Movie movie)
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

            return MovieQuery;

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

        public async Task<IEnumerable<MovieReviewDto>> GetMovieReviews(int movieApiId)
        {
            var movieQuery = await _dbContext.Movies.Include(m => m.MovieReviews).Where(m => m.MovieApiId == movieApiId).FirstOrDefaultAsync();

            List<MovieReviewDto> movieReviewList = await _dbContext.MovieReviews.Where(m => m.MovieId == movieQuery.MovieId)
                .Select(m => new MovieReviewDto()
                {
                    MovieReviewId = m.MovieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            return movieReviewList;
        }

        public async Task<IEnumerable<MovieReviewDto>> CreateMovieReviewList(CreateMovieReviewListRequest request)
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

            List<MovieReviewDto> movieReviewList = await _dbContext.MovieReviews.Where(m => m.MovieId == movieQuery.MovieId)
                .Select(m => new MovieReviewDto()
                {
                    MovieReviewId = m.MovieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            return movieReviewList;
        }

        public async Task<int> SetMovieRating(MovieRating movieRating)
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

            return ratingQuery.Rating;
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

        public async Task<int> getMovieDbId(int movieApiId)
        {
            var movie = await _dbContext.Movies.Where(m => m.MovieApiId == movieApiId).FirstOrDefaultAsync();

            return movie.MovieId;
        }
    }
}

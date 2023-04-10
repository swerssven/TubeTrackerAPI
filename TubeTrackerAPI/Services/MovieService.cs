using Newtonsoft.Json;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;
using System.Collections;
using TubeTrackerAPI.Models.Request;
using Azure.Core;

namespace TubeTrackerAPI.Services
{
    public class MovieService
    {
        private readonly TubeTrackerDbContext _dbContext;

        public MovieService(TubeTrackerDbContext dbContext)
        {
           _dbContext = dbContext;
        }

        public async Task<MovieResponse> GetMovieSearchList(string filter, int page, string language)
        {
            string resultStr = await new MovieRepository(this._dbContext).GetMovieSearchList(filter, page, language);

            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(resultStr);

            return movieResponse;  
        }

        public async Task<Movie> CreateMovie(int id, string language)
        {

            MovieRepository movieRepository = new MovieRepository(this._dbContext);
            string resultStr = await movieRepository.GetMovieExternal(id, language);

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
            movie.Trailer = trailer;
            movie.Poster = externalMovieDetailsResponse.poster_path;
            movie.Backdrop = externalMovieDetailsResponse.backdrop_path;
            movie.Duration = externalMovieDetailsResponse.runtime;

            if (language == "en-EN")
            {
                movie.TitleEn = externalMovieDetailsResponse.title;
                movie.DescriptionEn = externalMovieDetailsResponse.overview;
                movie.GenresEn = genres;
            } 
            else if (language == "es-ES")
            {
                movie.TitleEs = externalMovieDetailsResponse.title;
                movie.DescriptionEs = externalMovieDetailsResponse.overview;
                movie.GenresEs = genres;
            }

            return await movieRepository.CreateMovie(movie); 
        }

        public async Task<IEnumerable<MovieReviewDto>> GetMovieReviews(int movieApiId)
        {
            MovieRepository movieRepository = new MovieRepository(_dbContext);

            IEnumerable<MovieReviewDto> movieReviewResponse = await movieRepository.GetMovieReviews(movieApiId);

            return movieReviewResponse;
        }
        
        public async Task<IEnumerable<MovieReviewDto>> CreateMovieReviewList(CreateMovieReviewListRequest request)
        {
            MovieRepository movieRepository = new MovieRepository(_dbContext);

            IEnumerable<MovieReviewDto> movieReviewResponse = await movieRepository.CreateMovieReviewList(request);

            return movieReviewResponse;
        }
    }
}

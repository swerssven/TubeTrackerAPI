using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    public class SerieService
    {
        private readonly TubeTrackerDbContext _dbContext;
        private readonly SerieRepository _repository;

        public SerieService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new SerieRepository(dbContext);
        }

        public async Task<SerieResponse> GetSerieSearchList(string filter, int page, string language, int userId)
        {
            string resultStr = await new SerieRepository(this._dbContext).GetSerieSearchList(filter, page, language);

            SerieResponse serieResponse = JsonConvert.DeserializeObject<SerieResponse>(resultStr);

            serieResponse = await _repository.checkWatchedAndFavoriteSeriesFromList(serieResponse, userId);

            return serieResponse;
        }

        public async Task<SerieResponse> GetSeriePopularList(int page, string language)
        {
            string resultStr = await new SerieRepository(this._dbContext).GetSeriePopularList(page, language);

            SerieResponse serieResponse = JsonConvert.DeserializeObject<SerieResponse>(resultStr);

            return serieResponse;
        }

        public async Task<SerieResponse> GetSerieTopRatedList(string language)
        {
            string resultStr = await new SerieRepository(this._dbContext).GetSerieTopRatedList(language);

            SerieResponse serieResponse = JsonConvert.DeserializeObject<SerieResponse>(resultStr);

            return serieResponse;
        }

        public async Task<Series> CreateSerie(int id, string language)
        {

            SerieRepository serieRepository = new SerieRepository(this._dbContext);
            string resultSerieStr = await serieRepository.GetSerieExternal(id, language);
            ExternalSerieDetails externalSerieDetailsResponse = JsonConvert.DeserializeObject<ExternalSerieDetails>(resultSerieStr);

            List<SeasonsEpisode> seasonsEpisodesList = new List<SeasonsEpisode>();
            for (int numSeason = 1; numSeason <= externalSerieDetailsResponse.number_of_seasons; numSeason++)
            {
                string resultSeasonsEpisodesStr = await serieRepository.GetSeasonExternal(id, numSeason, language);
                ExternalSeasonDetails externalSeasonsDetails = JsonConvert.DeserializeObject<ExternalSeasonDetails>(resultSeasonsEpisodesStr);


                for (int numEpisode = 0; numEpisode < externalSeasonsDetails.episodes.Count(); numEpisode++)
                {
                    SeasonsEpisode seasonsEpisode = new SeasonsEpisode();
                    seasonsEpisode.NumSeason = numSeason;
                    seasonsEpisode.NumEpisode = externalSeasonsDetails.episodes.ElementAt(numEpisode).episode_number;
                    seasonsEpisode.EpisodeDuration = externalSeasonsDetails.episodes.ElementAt(numEpisode).runtime;
                    seasonsEpisode.PremiereDate = DateTimeOffset.Parse(externalSeasonsDetails.episodes.ElementAt(numEpisode).air_date).UtcDateTime;

                    if (language == "en-EN")
                    {
                        seasonsEpisode.TitleEpisodeEn = externalSeasonsDetails.episodes.ElementAt(numEpisode).name;
                    }
                    else if (language == "es-ES")
                    {
                        seasonsEpisode.TitleEpisodeEs = externalSeasonsDetails.episodes.ElementAt(numEpisode).name;
                    }
                    seasonsEpisodesList.Add(seasonsEpisode);
                }
            }

            List<SerieCast> cast = externalSerieDetailsResponse.credits.cast.Where(c => c.known_for_department == "Acting").ToList();
            string actors = null;
            if (cast != null)
            {
                actors = string.Join(", ", cast.Select(c => c.name).Take(20).ToList());
            }

            List<SerieCreatedBy> creators = externalSerieDetailsResponse.created_by.ToList();
            string directors = null;
            if (creators != null)
            {
                directors = string.Join(", ", creators.Select(c => c.name).Take(20).ToList());
            }

            List<SerieGenre> genreList = externalSerieDetailsResponse.genres.ToList();
            string genres = null;
            if (genreList != null)
            {
                genres = string.Join(", ", genreList.Select(g => g.name).Take(20).ToList());
            }

            Series serie = new Series();

            serie.SerieApiId = externalSerieDetailsResponse.id;
            serie.Actors = actors;
            serie.Creators = directors;
            serie.PremiereDate = DateTimeOffset.Parse(externalSerieDetailsResponse.first_air_date).UtcDateTime;
            serie.Poster = externalSerieDetailsResponse.poster_path;
            serie.Backdrop = externalSerieDetailsResponse.backdrop_path;

            if (language == "en-EN")
            {
                serie.TitleEn = externalSerieDetailsResponse.name;
                serie.DescriptionEn = externalSerieDetailsResponse.overview;
                serie.GenresEn = genres;
            }
            else if (language == "es-ES")
            {
                serie.TitleEs = externalSerieDetailsResponse.name;
                serie.DescriptionEs = externalSerieDetailsResponse.overview;
                serie.GenresEs = genres;
            }

            return await serieRepository.CreateSerie(serie, seasonsEpisodesList);
        }

        public async Task<IEnumerable<SerieReviewDto>> GetSerieReviews(int serieApiId)
        {
            SerieRepository serieRepository = new SerieRepository(_dbContext);

            IEnumerable<SerieReviewDto> serieReviewResponse = await serieRepository.GetSerieReviews(serieApiId);

            return serieReviewResponse;
        }

        public async Task<IEnumerable<SerieReviewDto>> CreateSerieReviewList(CreateSerieReviewListRequest request)
        {
            SerieRepository serieRepository = new SerieRepository(_dbContext);

            IEnumerable<SerieReviewDto> serieReviewResponse = await serieRepository.CreateSerieReviewList(request);

            return serieReviewResponse;
        }

        public async Task<RatingsDto> SetSerieRating(int serieApiId, int userId, int rating)
        {
            SerieRepository serieRepository = new SerieRepository(_dbContext);

            SerieRating serieRating = new SerieRating();

            serieRating.SerieId = await serieRepository.getSerieDbId(serieApiId);
            serieRating.UserId = userId;
            serieRating.Rating = rating;

            return await serieRepository.SetSerieRating(serieRating);
        }

        public async Task<RatingsDto> GetSerieRatings(int userId, int serieApiId)
        {
            SerieRepository serieRepository = new SerieRepository(_dbContext);

            return await serieRepository.GetSerieRatings(userId, serieApiId);
        }

        public async Task<bool> setSerieWatched(int serieApiId, int userId, string language, bool watched)
        {
            var serieId = await _repository.getSerieDbId(serieApiId);

            if (serieId == 0) // Check if serie in database, if not, it will be created.
            {
                await this.CreateSerie(serieApiId, language);
                serieId = await _repository.getSerieDbId(serieApiId);
            }
            return await _repository.setSerieWatched(serieId, userId, watched);
        }
    }
}

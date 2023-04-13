using Newtonsoft.Json;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    public class SerieService
    {
        private readonly TubeTrackerDbContext _dbContext;

        public SerieService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SerieResponse> GetSerieSearchList(string filter, int page, string language)
        {
            string resultStr = await new SerieRepository(this._dbContext).GetSerieSearchList(filter, page, language);

            SerieResponse serieResponse = JsonConvert.DeserializeObject<SerieResponse>(resultStr);

            return serieResponse;
        }

        public async Task<Series> CreateSerie(int id, string language)
        {

            SerieRepository serieRepository = new SerieRepository(this._dbContext);
            string resultStr = await serieRepository.GetSerieExternal(id, language);

            ExternalSerieDetails externalSerieDetailsResponse = JsonConvert.DeserializeObject<ExternalSerieDetails>(resultStr);

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

            return await serieRepository.CreateSerie(serie);
        }
    }
}

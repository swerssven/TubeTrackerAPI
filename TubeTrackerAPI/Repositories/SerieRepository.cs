using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    public class SerieRepository
    {
        private const string URL = "https://api.themoviedb.org/3";
        private const string apiKey = "7d22105ae1b958ce88fe42db67a97318";
        private readonly TubeTrackerDbContext _dbContext;

        public SerieRepository(TubeTrackerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        // Gets a list of series with the necesary information from external API to make serie-cards
        public async Task<string> GetSerieSearchList(string filter, int page, string language)
        {
            string apiURL = $"/search/tv?api_key={apiKey}&language={language}&query={filter}&page={page}&include_adult=false";

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

        // Create new serie in data base
        public async Task<Series> CreateSerie(Series serie)
        {
            var SerieQuery = await _dbContext.Series.Where(m => m.SerieApiId == serie.SerieApiId).FirstOrDefaultAsync();
            //var SerieQuery = await _dbContext.Series.Include(s => s.SeasonsEpisodes).Where(m => m.SerieApiId == serie.SerieApiId).FirstOrDefaultAsync();

            if (SerieQuery == null)
            {
                _dbContext.Series.Add(serie);
                await _dbContext.SaveChangesAsync();
                SerieQuery = serie;
            }
            else if (SerieQuery.TitleEn == null && serie.TitleEn != null)
            {
                SerieQuery.TitleEn = serie.TitleEn;
                SerieQuery.DescriptionEn = serie.DescriptionEn;
                SerieQuery.GenresEn = serie.GenresEn;
                _dbContext.Series.Update(SerieQuery);
                await _dbContext.SaveChangesAsync();
            }
            else if (SerieQuery.TitleEs == null && serie.TitleEs != null)
            {
                SerieQuery.TitleEs = serie.TitleEs;
                SerieQuery.DescriptionEs = serie.DescriptionEs;
                SerieQuery.GenresEs = serie.GenresEs;
                _dbContext.Series.Update(SerieQuery);
                await _dbContext.SaveChangesAsync();
            }

            return SerieQuery;

        }

        // Gets a serie from external API
        public async Task<string> GetSerieExternal(int id, string language)
        {
            string apiURL = $"/tv/{id}?api_key={apiKey}&language={language}&append_to_response=videos,credits";

            //SeasonsEpisode = await _dbContext.Series.Include(s => s.SeasonsEpisodes).Where(s => s.SerieApiId == id)

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URL + apiURL))
                {
                    string apiResponse = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                    //string prueba = await GetSeasonsExternal(1100, "es-ES", 2);
                    return apiResponse;
                }
            }
        }
        /*
        // Gets a seasons from external API
        public async Task<string> GetSeasonsExternal(int id, string language, int numOfSeasons)
        {
            List<string> seasonsList = new List<string>();

            for (int i = 1; i <= numOfSeasons; i++)
            {

                string apiURL = $"/tv/{id}/season/{i}?api_key={apiKey}&language={language}";

                //SeasonsEpisode = await _dbContext.Series.Include(s => s.SeasonsEpisodes).Where(s => s.SerieApiId == id)

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(URL + apiURL))
                    {
                        string apiResponse = string.Empty;

                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                        }

                        seasonsList.Add(apiResponse);
                    }
                }

            }

            string seasonsStringResponse = (JsonConvert.SerializeObject(seasonsList));
            var list = JsonConvert.DeserializeObject(seasonsStringResponse);

            return seasonsStringResponse;
        }*/
    }
}

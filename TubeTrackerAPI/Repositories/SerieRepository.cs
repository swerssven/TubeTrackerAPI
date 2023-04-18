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
        public async Task<Series> CreateSerie(Series serie, List<SeasonsEpisode> seasonsEpisodes)
        {
            var SerieQuery = await _dbContext.Series.Include(m => m.SeasonsEpisodes).Where(m => m.SerieApiId == serie.SerieApiId).FirstOrDefaultAsync();

            if (SerieQuery == null)
            {
                _dbContext.Series.Add(serie);
                await _dbContext.SaveChangesAsync();
                SerieQuery = serie;

                foreach (var season in seasonsEpisodes)
                {
                    season.SerieId = SerieQuery.SerieId;
                    _dbContext.SeasonsEpisodes.Add(season);
                }

                await _dbContext.SaveChangesAsync();
            }
            else if (SerieQuery.TitleEn == null && serie.TitleEn != null)
            {
                //var SeasonQuery = await _dbContext.SeasonsEpisodes.Where(s => s.SerieId == SerieQuery.SerieId).ToListAsync();

                SerieQuery.TitleEn = serie.TitleEn;
                SerieQuery.DescriptionEn = serie.DescriptionEn;
                SerieQuery.GenresEn = serie.GenresEn;

                foreach (var episode in SerieQuery.SeasonsEpisodes)
                {
                    var episodeExtApi = seasonsEpisodes.FirstOrDefault(e => e.NumSeason == episode.NumSeason && e.NumEpisode == episode.NumEpisode);

                    if(episodeExtApi != null)
                    {
                        episode.TitleEpisodeEn = episodeExtApi.TitleEpisodeEn;
                    }
                }

                _dbContext.Series.Update(SerieQuery);
                await _dbContext.SaveChangesAsync();
            }
            else if (SerieQuery.TitleEs == null && serie.TitleEs != null)
            {
                SerieQuery.TitleEs = serie.TitleEs;
                SerieQuery.DescriptionEs = serie.DescriptionEs;
                SerieQuery.GenresEs = serie.GenresEs;

                foreach (var episode in SerieQuery.SeasonsEpisodes)
                {
                    var episodeExtApi = seasonsEpisodes.FirstOrDefault(e => e.NumSeason == episode.NumSeason && e.NumEpisode == episode.NumEpisode);

                    if (episodeExtApi != null)
                    {
                        episode.TitleEpisodeEs = episodeExtApi.TitleEpisodeEs;
                    }
                }

                _dbContext.Series.Update(SerieQuery);
                await _dbContext.SaveChangesAsync();
            }

            return SerieQuery;
        }

        public async Task<string> GetSeasonExternal(int serieId, int numSeason, string language)
        {
            string apiURL = $"/tv/{serieId}/season/{numSeason}?api_key={apiKey}&language={language}";

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

        // Gets a serie from external API
        public async Task<string> GetSerieExternal(int id, string language)
        {
            string apiURL = $"/tv/{id}?api_key={apiKey}&language={language}&append_to_response=videos,credits";

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
    }
}

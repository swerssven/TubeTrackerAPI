using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
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

        public async Task<string> GetSeriePopularList(int page, string language)
        {
            Random rnd = new Random();
            string apiURL = $"/discover/tv?api_key={apiKey}&with_original_language=en&sort_by=popularity.desc&language={language}&page={rnd.Next(1, 10)}";

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

        public async Task<string> GetSerieTopRatedList(string language)
        {
            Random rnd = new Random();
            string apiURL = $"/tv/top_rated?api_key={apiKey}&language={language}&page={rnd.Next(1, 10)}";

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

        public async Task<IEnumerable<SerieReviewDto>> GetSerieReviews(int serieApiId)
        {
            var serieQuery = await _dbContext.Series.Include(m => m.SerieReviews).Where(m => m.SerieApiId == serieApiId).FirstOrDefaultAsync();

            List<SerieReviewDto> serieReviewList = await _dbContext.SerieReviews.Where(m => m.SerieId == serieQuery.SerieId)
                .Select(m => new SerieReviewDto()
                {
                    SerieReviewId = m.SerieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            return serieReviewList;
        }

        public async Task<IEnumerable<SerieReviewDto>> CreateSerieReviewList(CreateSerieReviewListRequest request)
        {
            var serieQuery = await _dbContext.Series.Include(m => m.SerieReviews).Where(m => m.SerieApiId == request.SerieApiId).FirstOrDefaultAsync();
            var reviewQuery = serieQuery.SerieReviews.Where(m => m.UserId == request.UserId).FirstOrDefault();

            SerieReview serieReview = new SerieReview();
            serieReview.Content = request.Content;
            serieReview.UserId = request.UserId;
            serieReview.CreationDate = DateTime.UtcNow;

            if (reviewQuery == null && request.Content != null)
            {
                serieReview.SerieId = serieQuery.SerieId;
                _dbContext.SerieReviews.Add(serieReview);
                await _dbContext.SaveChangesAsync();
            }
            else if (reviewQuery != null)
            {
                reviewQuery.Content = serieReview.Content;
                reviewQuery.CreationDate = serieReview.CreationDate;
                _dbContext.SerieReviews.Update(reviewQuery);
                await _dbContext.SaveChangesAsync();
            }

            List<SerieReviewDto> serieReviewList = await _dbContext.SerieReviews.Where(m => m.SerieId == serieQuery.SerieId)
                .Select(m => new SerieReviewDto()
                {
                    SerieReviewId = m.SerieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            return serieReviewList;
        }

        public async Task<RatingsDto> SetSerieRating(SerieRating serieRating)
        {
            var ratingQuery = _dbContext.SerieRatings.Where(r => r.SerieId == serieRating.SerieId && r.UserId == serieRating.UserId).FirstOrDefault();

            if (ratingQuery == null)
            {
                _dbContext.SerieRatings.Add(serieRating);
                await _dbContext.SaveChangesAsync();
                ratingQuery = serieRating;
            }
            else
            {
                ratingQuery.Rating = serieRating.Rating;
                _dbContext.SerieRatings.Update(ratingQuery);
                await _dbContext.SaveChangesAsync();
            }

            return await GetSerieRatings(serieRating.UserId, ratingQuery.Serie.SerieApiId);
        }

        public async Task<RatingsDto> GetSerieRatings(int userId, int serieApiId)
        {
            RatingsDto ratingsDto = new RatingsDto();
            int serieId = await getSerieDbId(serieApiId);
            var RatingQuery = await _dbContext.SerieRatings.Where(r => r.SerieId == serieId).ToListAsync();

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

        public async Task<int> getSerieDbId(int serieApiId)
        {
            var serie = await _dbContext.Series.Where(m => m.SerieApiId == serieApiId).FirstOrDefaultAsync();

            return serie.SerieId;
        }
    }
}

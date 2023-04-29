using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
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

        public async Task<string> GetSeriePopularList(string language)
        {
            Random rnd = new Random();
            string apiURL = $"/discover/tv?api_key={apiKey}&with_original_language=en&sort_by=popularity.desc&language={language}&page={rnd.Next(1, 5)}";

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
            string apiURL = $"/tv/top_rated?api_key={apiKey}&language={language}&page={rnd.Next(1, 5)}";

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
        public async Task<SerieDto> CreateSerie(Series serie, List<SeasonsEpisode> seasonsEpisodes, int userId)
        {
            var SerieQuery = await _dbContext.Series.Where(m => m.SerieApiId == serie.SerieApiId).FirstOrDefaultAsync();

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

            SerieDto serieDto = new SerieDto();
            serieDto.SerieId = SerieQuery.SerieId;
            serieDto.SerieApiId = SerieQuery.SerieApiId;
            serieDto.TitleEn = SerieQuery.TitleEn;
            serieDto.TitleEs = SerieQuery.TitleEs;
            serieDto.DescriptionEn = SerieQuery.DescriptionEn;
            serieDto.DescriptionEs = SerieQuery.DescriptionEs;
            serieDto.Actors = SerieQuery.Actors;
            serieDto.Creators = SerieQuery.Creators;
            serieDto.GenresEn = SerieQuery.GenresEn;
            serieDto.GenresEs = SerieQuery.GenresEs;
            serieDto.PremiereDate = SerieQuery.PremiereDate;
            serieDto.Poster = SerieQuery.Poster;
            serieDto.Backdrop = SerieQuery.Backdrop;
            await this.checkWatchedAndFavoriteSerie(serieDto, userId);

            return serieDto;
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

        public async Task<SerieReviewDto> GetSerieReviews(int serieApiId)
        {
            var serieQuery = await _dbContext.Series.Include(m => m.SerieReviews).Where(m => m.SerieApiId == serieApiId).FirstOrDefaultAsync();

            SerieReviewDto serieReview = new SerieReviewDto();

            serieReview.reviews = await _dbContext.SerieReviews.Where(m => m.SerieId == serieQuery.SerieId)
                .Select(m => new SerieReviewItemDto()
                {
                    SerieReviewId = m.SerieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            serieReview.numReviews = await _dbContext.SerieReviews.CountAsync(m => m.SerieId == serieQuery.SerieId);

            return serieReview;
        }

        public async Task<SerieReviewDto> CreateSerieReviewList(CreateSerieReviewListRequest request)
        {
            var serieQuery = await _dbContext.Series.Include(m => m.SerieReviews).Where(m => m.SerieApiId == request.SerieApiId).FirstOrDefaultAsync();
            var reviewQuery = serieQuery.SerieReviews.Where(m => m.UserId == request.UserId).FirstOrDefault();

            SerieReview review = new SerieReview();
            review.Content = request.Content;
            review.UserId = request.UserId;
            review.CreationDate = DateTime.UtcNow;

            if (reviewQuery == null && request.Content != null)
            {
                review.SerieId = serieQuery.SerieId;
                _dbContext.SerieReviews.Add(review);
                await _dbContext.SaveChangesAsync();
            }
            else if (reviewQuery != null)
            {
                reviewQuery.Content = review.Content;
                reviewQuery.CreationDate = review.CreationDate;
                _dbContext.SerieReviews.Update(reviewQuery);
                await _dbContext.SaveChangesAsync();
            }

            SerieReviewDto serieReview = new SerieReviewDto();

            serieReview.reviews = await _dbContext.SerieReviews.Where(m => m.SerieId == serieQuery.SerieId)
                .Select(m => new SerieReviewItemDto()
                {
                    SerieReviewId = m.SerieReviewId,
                    Content = m.Content,
                    UserId = m.UserId,
                    UserNickname = m.User.Nickname,
                    UserImage = m.User.Image,
                    CreationDate = m.CreationDate
                }).ToListAsync();

            serieReview.numReviews = await _dbContext.SerieReviews.CountAsync(m => m.SerieId == serieQuery.SerieId);

            return serieReview;
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

        public async Task<bool> setSerieWatched(int serieId, int userId, bool watched)
        {
            bool result = watched;

            if (watched) // If true we save watched episodes.
            {

                List<SeasonsEpisode> episodes = await _dbContext.SeasonsEpisodes.Where(s => s.SerieId == serieId).ToListAsync();
                List<WatchedSeriesSeasonsEpisode> watchedEpisodes = await _dbContext.WatchedSeriesSeasonsEpisodes.Where(s => s.SerieId == serieId && s.UserId == userId).ToListAsync();

                foreach (var episode in episodes)
                {
                    if (!watchedEpisodes.Any(s => s.SeasonsEpisodesId == episode.SeasonsEpisodesId))
                    {
                        WatchedSeriesSeasonsEpisode watchedSeriesSeasonsEpisode = new WatchedSeriesSeasonsEpisode();
                        watchedSeriesSeasonsEpisode.SerieId = serieId;
                        watchedSeriesSeasonsEpisode.UserId = userId;
                        watchedSeriesSeasonsEpisode.DateWatched = DateTime.Now;
                        watchedSeriesSeasonsEpisode.SeasonsEpisodesId = episode.SeasonsEpisodesId;
                        _dbContext.WatchedSeriesSeasonsEpisodes.Add(watchedSeriesSeasonsEpisode);
                    }   
                }

                await _dbContext.SaveChangesAsync();
                result = true;
            }
            else if (!watched)  // If false we remove watched episodes.
            { 
                List<WatchedSeriesSeasonsEpisode> watchedEpisodes = await _dbContext.WatchedSeriesSeasonsEpisodes.Where(w => w.SerieId == serieId && w.UserId == userId).ToListAsync();
                _dbContext.WatchedSeriesSeasonsEpisodes.RemoveRange(watchedEpisodes);
                await _dbContext.SaveChangesAsync();
                result = false;
            }

            return result;
        }

        public async Task<bool> setSeasonEpisodeWatched(int serieId, int seasonsEpisodeId, int userId, bool watched)
        {
            bool result = watched;

            if (watched)
            {
                WatchedSeriesSeasonsEpisode watchedSeriesSeasonsEpisode = new WatchedSeriesSeasonsEpisode();
                watchedSeriesSeasonsEpisode.SerieId = serieId;
                watchedSeriesSeasonsEpisode.SeasonsEpisodesId = seasonsEpisodeId;
                watchedSeriesSeasonsEpisode.UserId = userId;
                watchedSeriesSeasonsEpisode.DateWatched = DateTime.Now;

                _dbContext.WatchedSeriesSeasonsEpisodes.Add(watchedSeriesSeasonsEpisode);
                await _dbContext.SaveChangesAsync();
                result = true;
            }
            else if (!watched)
            {
                var watchedSeriesSeasonsEpisodeQuery = await _dbContext.WatchedSeriesSeasonsEpisodes
                    .Where(w => w.SerieId == serieId && w.SeasonsEpisodesId == seasonsEpisodeId && w.UserId == userId)
                    .FirstOrDefaultAsync();
                _dbContext.WatchedSeriesSeasonsEpisodes.Remove(watchedSeriesSeasonsEpisodeQuery);
                await _dbContext.SaveChangesAsync();
                result = false;
            }

            return result;
        }

        // Mark/unmark movie as favorite.
        public async Task<bool> setSerieFavorite(int serieId, int userId, bool isFavorite)
        {
            bool result = isFavorite;

            if (isFavorite)
            {
                FavoriteSeries favoriteSerie = new FavoriteSeries();
                favoriteSerie.SerieId = serieId;
                favoriteSerie.UserId = userId;
                favoriteSerie.DateAdded = DateTime.Now;

                _dbContext.FavoriteSeries.Add(favoriteSerie);
                await _dbContext.SaveChangesAsync();
                result = true;
            }
            else if (!isFavorite)
            {
                var favoriteSerieQuery = await _dbContext.FavoriteSeries.Where(f => f.SerieId == serieId && f.UserId == userId).FirstOrDefaultAsync();
                _dbContext.FavoriteSeries.Remove(favoriteSerieQuery);
                await _dbContext.SaveChangesAsync();
                result = false;
            }

            return result;
        }

        public async Task<int> getSerieDbId(int serieApiId)
        {
            var serie = await _dbContext.Series.Where(m => m.SerieApiId == serieApiId).FirstOrDefaultAsync();

            if (serie == null)
            {
                return 0;
            }

            return serie.SerieId;
        }

        // Check if list of serie is watched and favorited by specific user.
        public async Task<SerieResponse> checkWatchedAndFavoriteSeriesFromList(SerieResponse serieResponse, int userId)
        {
            foreach (var serie in serieResponse.Results)
            {
                var serieId = await this.getSerieDbId(serie.id);

                int episodeCount = await _dbContext.SeasonsEpisodes.CountAsync(s => s.SerieId == serieId);

                int watchedEpisodeCount = await _dbContext.WatchedSeriesSeasonsEpisodes.CountAsync(w => w.SerieId == serieId && w.UserId == userId);

                if (episodeCount == watchedEpisodeCount && episodeCount > 0)
                {
                    serie.watched = true;
                }
                else
                {
                    serie.watched = false;
                }

                var favoriteQuery = await _dbContext.FavoriteSeries.Where(w => w.SerieId == serieId && w.UserId == userId).FirstOrDefaultAsync();

                if (favoriteQuery != null)
                {
                    serie.favorite = true;
                }
                else
                {
                    serie.favorite = false;
                }
            }

            return serieResponse;
        }

        // Check if serie is watched and favorited by specific user.
        public async Task<SerieDto> checkWatchedAndFavoriteSerie(SerieDto serie, int userId)
        {
            var serieId = await this.getSerieDbId(serie.SerieApiId);

            int episodeCount = await _dbContext.SeasonsEpisodes.CountAsync(s => s.SerieId == serieId);

            int watchedEpisodeCount = await _dbContext.WatchedSeriesSeasonsEpisodes.CountAsync(w => w.SerieId == serieId && w.UserId == userId);

            if (episodeCount == watchedEpisodeCount && episodeCount > 0)
            {
                serie.watched = true;
            }
            else
            {
                serie.watched = false;
            }

            var favoriteQuery = await _dbContext.FavoriteSeries.Where(w => w.SerieId == serieId && w.UserId == userId).FirstOrDefaultAsync();

            if (favoriteQuery != null)
            {
                serie.favorite = true;
            }
            else
            {
                serie.favorite = false;
            }

            return serie;
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
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

        public async Task<SerieResponse> GetSeriePopularList()
        {
            SerieResponse serieResponse = new SerieResponse();
            serieResponse.Results = new List<ExternalSerie>();
            var query = @"
                SELECT TOP 20 s.*, 
                    ISNULL(f.FavoriteCount, 0) AS FavoriteCount, 
                    ISNULL(w.WatchedCount, 0) AS WatchedCount
                FROM Series AS s
                LEFT JOIN (
                    SELECT SerieId, COUNT(*) AS FavoriteCount
                    FROM FavoriteSeries
                    GROUP BY SerieId
                ) AS f ON s.SerieId = f.SerieId
                LEFT JOIN (
                    SELECT SerieId, COUNT(*) AS WatchedCount
                    FROM WatchedSeriesSeasonsEpisodes
                    GROUP BY SerieId
                ) AS w ON s.SerieId = w.SerieId
                ORDER BY ISNULL(f.FavoriteCount, 0) DESC, ISNULL(w.WatchedCount, 0) DESC";

            var popularSeries = await _dbContext.Series
                .FromSqlRaw(query)
                .ToListAsync();

            foreach (var m in popularSeries)
            {
                ExternalSerie externalSerie = new ExternalSerie()
                {
                    id = m.SerieApiId,
                    name = (m.TitleEn != null ? m.TitleEn : m.TitleEs),
                    first_air_date = (m.PremiereDate).ToString(),
                    poster_path = m.Poster,
                    backdrop_path = m.Backdrop
                };
                serieResponse.Results.Add(externalSerie);
            }

            return serieResponse;
        }

        public async Task<SerieResponse> GetSerieTopRatedList()
        {
            SerieResponse serieResponse = new SerieResponse();
            serieResponse.Results = new List<ExternalSerie>();
            var query = @"
                SELECT TOP 20 m.*, 
                    ISNULL(r.RatingCount, 0) AS RatingCount
                FROM Series AS m
                LEFT JOIN (
                    SELECT SerieId, COUNT(*) AS RatingCount
                    FROM SerieRatings
                    GROUP BY SerieId
                ) AS r ON m.SerieId = r.SerieId
                ORDER BY ISNULL(r.RatingCount, 0) DESC";

            var topRatedSeries = await _dbContext.Series
                .FromSqlRaw(query)
                .ToListAsync();

            foreach (var m in topRatedSeries)
            {
                ExternalSerie externalSerie = new ExternalSerie()
                {
                    id = m.SerieApiId,
                    name = (m.TitleEn != null ? m.TitleEn : m.TitleEs),
                    first_air_date = (m.PremiereDate).ToString(),
                    poster_path = m.Poster,
                    backdrop_path = m.Backdrop
                };
                serieResponse.Results.Add(externalSerie);
            }

            return serieResponse;
        }

        // Create new serie in data base
        public async Task<SerieDto> CreateSerie(Series serie, List<SeasonsEpisode> seasonsEpisodes, int userId)
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

        public async Task<SeasonsEpisodesListDto> GetSeasonsEpisodesList(int serieApiId, int userId)
        {
            int serieId = await this.getSerieDbId(serieApiId);

            SeasonsEpisodesListDto seasonsEpisodesListDto = new SeasonsEpisodesListDto();

            seasonsEpisodesListDto.TotalNumSeasons = await _dbContext.SeasonsEpisodes.Where(s => s.SerieId == serieId).Select(s => s.NumSeason).Distinct().CountAsync();

            seasonsEpisodesListDto.TotalNumEpisodes = await _dbContext.SeasonsEpisodes.Where(s => s.SerieId == serieId).Select(s => s.NumEpisode).CountAsync();

            seasonsEpisodesListDto.SeasonsList = new List<SeasonsDto>();

            for (int i = 1; i <= seasonsEpisodesListDto.TotalNumSeasons; i++)
            {
                SeasonsDto seasonsDto = new SeasonsDto();
                seasonsDto.NumSeason = i;
                seasonsDto.EpisodesList = await _dbContext.SeasonsEpisodes.Where(s => s.SerieId == serieId && s.NumSeason == i)
                    .Select(s => new SeasonsEpisodeDto
                    {
                        SeasonsEpisodesId = s.SeasonsEpisodesId,
                        SerieApiId = serieApiId,
                        NumSeason = s.NumSeason,
                        NumEpisode = s.NumEpisode,
                        EpisodeDuration = s.EpisodeDuration,
                        TitleEpisodeEn = s.TitleEpisodeEn,
                        TitleEpisodeEs = s.TitleEpisodeEs,
                        PremiereDate = s.PremiereDate
                    }).ToListAsync();
                seasonsDto = await this.checkWatchedSeasonEpisodeFromList(seasonsDto, serieId, userId);
                seasonsEpisodesListDto.SeasonsList.Add(seasonsDto);
            }

            return seasonsEpisodesListDto;
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
                    CreationDate = m.CreationDate,
                    Rating = _dbContext.SerieRatings.Where(x => x.UserId == m.UserId && x.SerieId == m.SerieId).Select(x => x.Rating).FirstOrDefault()
                }).ToListAsync();

            serieReview.numReviews = await _dbContext.SerieReviews.CountAsync(m => m.SerieId == serieQuery.SerieId);

            return serieReview;
        }

        public async Task<bool> DeleteSerieReview(int serieReviewId)
        {
            var querySerieReview = await _dbContext.SerieReviews.Where(m => m.SerieReviewId == serieReviewId).FirstOrDefaultAsync();
            if (querySerieReview != null)
            {
                _dbContext.Remove(querySerieReview);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            };
            return false; ;
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
                string formattedNumber = (RatingQuery.Average(r => r.Rating)).ToString("F1");
                ratingsDto.AverageRating = double.Parse(formattedNumber);
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

        // Get user's favorites series.
        public async Task<IEnumerable<ExternalSerie>> getSeriesFavoritesList(int userId, string language)
        {
            List<int> serieIds = await _dbContext.FavoriteSeries.Where(f => f.UserId == userId).Select(s => s.SerieId).ToListAsync();
            IEnumerable<ExternalSerie> favoriteSerieList = new List<ExternalSerie>();

            if(language == "en-EN")
            {
                favoriteSerieList = await _dbContext.Series.Where(s => serieIds.Contains(s.SerieId))
                    .Select(s => new ExternalSerie()
                    {
                        id = s.SerieApiId,
                        name = s.TitleEn,
                        first_air_date = s.PremiereDate.ToString(),
                        poster_path = s.Poster,
                        backdrop_path = s.Backdrop,
                    }).ToListAsync();
            }
            else if(language == "es-ES")
            {
                favoriteSerieList = await _dbContext.Series.Where(s => serieIds.Contains(s.SerieId))
                    .Select(s => new ExternalSerie()
                    {
                        id = s.SerieApiId,
                        name = s.TitleEs,
                        first_air_date = s.PremiereDate.ToString(),
                        poster_path = s.Poster,
                        backdrop_path = s.Backdrop,
                    }).ToListAsync();
            }

            foreach (var serie in favoriteSerieList)
            {
                await checkWatchedAndFavoriteSerie(serie, userId);
                serie.DateAddedFavorite = await _dbContext.FavoriteSeries.Where(f => f.UserId == userId && f.SerieId == serie.id).
                    Select(s => s.DateAdded).FirstOrDefaultAsync();
            }

            return favoriteSerieList;
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

        // Check if serie is watched and favorited by specific user with serie dto.
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

        // Check if episode is watched by specific user.
        public async Task<SeasonsDto> checkWatchedSeasonEpisodeFromList(SeasonsDto seasonsDto, int serieId, int userId)
        {
            foreach (var episode in seasonsDto.EpisodesList)
            {
                var watchedQuery = await _dbContext.WatchedSeriesSeasonsEpisodes.Where(w => w.SerieId == serieId && w.SeasonsEpisodesId == episode.SeasonsEpisodesId && w.UserId == userId).FirstOrDefaultAsync();
                
                if (watchedQuery != null)
                {
                    episode.Watched = true;
                }
                else
                {
                    episode.Watched = false;
                }
            }

            return seasonsDto;
        }

        // Check if serie is watched and favorited by specific user with external serie.
        public async Task<ExternalSerie> checkWatchedAndFavoriteSerie(ExternalSerie serie, int userId)
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

            return serie;
        }

        // Get user's last 10 watched series.
        public async Task<IEnumerable<ExternalSerie>> getLastWatchedSeriesList(int userId, string language)
        {
            List<int> serieIds = await _dbContext.WatchedSeriesSeasonsEpisodes.OrderByDescending(e => e.DateWatched).Where(f => f.UserId == userId).Select(s => s.SerieId).Distinct().ToListAsync();
            IEnumerable<ExternalSerie> watchedSerieList = new List<ExternalSerie>();

            if (language == "en-EN")
            {
                watchedSerieList = await _dbContext.Series.Where(s => serieIds.Contains(s.SerieId))
                    .Select(s => new ExternalSerie()
                    {
                        id = s.SerieApiId,
                        name = s.TitleEn,
                        first_air_date = s.PremiereDate.ToString(),
                        poster_path = s.Poster,
                        backdrop_path = s.Backdrop,
                    }).ToListAsync();
            }
            else if (language == "es-ES")
            {
                watchedSerieList = await _dbContext.Series.Where(s => serieIds.Contains(s.SerieId))
                    .Select(s => new ExternalSerie()
                    {
                        id = s.SerieApiId,
                        name = s.TitleEs,
                        first_air_date = s.PremiereDate.ToString(),
                        poster_path = s.Poster,
                        backdrop_path = s.Backdrop,
                    }).ToListAsync();
            }

            foreach (var serie in watchedSerieList)
            {
                await checkWatchedAndFavoriteSerie(serie, userId);
                serie.DateAddedFavorite = await _dbContext.FavoriteSeries.Where(f => f.UserId == userId && f.SerieId == serie.id).
                    Select(s => s.DateAdded).FirstOrDefaultAsync();
            }

            return watchedSerieList.Take(6);
        }
    }
}

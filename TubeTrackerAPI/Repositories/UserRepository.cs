using MessagePack.Formatters;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Services;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    internal class UserRepository
    {
        private readonly TubeTrackerDbContext _dbContext;

        internal UserRepository(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        internal async Task<int?> GetUserId(string username, string password)
        {
            int? result = null;

            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(username) && x.Password.Equals(password));

            if (user != null)
            {
                result = user.UserId;
            }

            return result;
        }

        internal async Task<UserDto> GetUser(int id)
        {
            User user = await _dbContext.Users.FindAsync(id);
            UserDto userResponse = new UserDto();

            userResponse.UserId = user.UserId;
            userResponse.FirstName = user.FirstName;
            userResponse.LastName = user.LastName;
            userResponse.Nickname = user.Nickname;
            userResponse.Email = user.Email;
            userResponse.Language = user.Language;
            userResponse.Image = user.Image;
            userResponse.RolId = user.RolId;

            return userResponse;
        }

        internal async Task<List<usersGridDto>> GetUserList()
        {
            List<usersGridDto> userResponse = new List<usersGridDto>();
            userResponse = await _dbContext.Users.Select(
                u => new usersGridDto()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Nickname = u.Nickname,
                    Email = u.Email,
                    IsAdmin = !Convert.ToBoolean(u.RolId)
                }).ToListAsync();

            return userResponse;
        }

        internal async Task CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        internal async Task<bool> CheckUserEmail(int userId, string email)
        {
            return await _dbContext.Users.AnyAsync(a => a.Email == email && a.UserId == userId);
        }

        internal async Task<bool> CheckEmail(string email)
        {
            return await _dbContext.Users.AnyAsync(a => a.Email == email);
        }
        internal async Task<bool> CheckUserNickname(int userId, string nickname)
        {
            return await _dbContext.Users.AnyAsync(a => a.Nickname == nickname && a.UserId == userId);
        }

        internal async Task<bool> CheckNickname(string nickname)
        {
            return await _dbContext.Users.AnyAsync(a => a.Nickname == nickname);
        }

        internal async Task<UserStatisticsDto> GetUserStatistics(int userId)
        {
            UserStatisticsDto userStatisticsDto = new UserStatisticsDto();

            var tmpSum = await _dbContext.WatchedSeriesSeasonsEpisodes.Include(w => w.SeasonsEpisodes).Where(w => w.UserId == userId).SumAsync(s => s.SeasonsEpisodes.EpisodeDuration);

            if(tmpSum != null)
            {
                userStatisticsDto.TotalHoursSeries = (int)tmpSum / 60;
            } 

            tmpSum = await _dbContext.WatchedMovies.Include(w => w.Movie).Where(w => w.UserId == userId).SumAsync(s => s.Movie.Duration);

            if(tmpSum != null)
            {
                userStatisticsDto.TotalHoursMovies = (int)tmpSum / 60;
            }

            userStatisticsDto.WatchedEpisodes = await _dbContext.WatchedSeriesSeasonsEpisodes.Where(w => w.UserId == userId).CountAsync();

            userStatisticsDto.WatchedMovies = await _dbContext.WatchedMovies.Where(w => w.UserId == userId).CountAsync();

            userStatisticsDto.Posts = await _dbContext.Posts.Where(p => p.UserId == userId).CountAsync();

            userStatisticsDto.PostComments = await _dbContext.PostComments.Where(w => w.UserId == userId).CountAsync();
            
            var ss = await _dbContext.Users.Include(x => x.PostsNavigation).FirstOrDefaultAsync(x => x.UserId == userId);

            userStatisticsDto.LikesPosts = ss.PostsNavigation.Count();

            userStatisticsDto.Friends = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendshipStatus == (int)FriendshipStatus.Friends).CountAsync();

            return userStatisticsDto;
        }

        internal async Task<UserDto> EditUser(EditUserRequest user)
        {
            var query = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            UserDto userResponse = new UserDto();

            if (query != null)
            {
                query.Nickname = user.Nickname;
                query.Password = user.Password;
                query.Email = user.Email;
                query.Language = user.Language;
                query.Image = user.Image;

                _dbContext.Update(query);
                await _dbContext.SaveChangesAsync();

                userResponse = new UserDto()
                {
                    UserId = query.UserId,
                    FirstName = query.FirstName,
                    LastName = query.LastName,
                    Nickname = query.Nickname,
                    Email = query.Email,
                    Language = query.Language,
                    Image = query.Image,
                    RolId = query.RolId
                };
            }

            return userResponse;
        }

        internal async Task<bool> MakeUserAdmin(int userId, bool isAdmin)
        {
            var queryAdmin = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if(queryAdmin != null)
            {
                if (isAdmin)
                {
                    queryAdmin.RolId = (int)RolEnum.Admin;
                }
                else
                {
                    queryAdmin.RolId = (int)RolEnum.User;
                }
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            }
                
            return false;
        }

        internal async Task<bool> ChangeUserState(int userId, bool isActive)
        {
            var queryUserState = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (queryUserState != null)
            {
                if (isActive)
                {
                    //queryUserState.isActive = (int)UserState.Active;
                }
                else
                {
                    //queryUserState.isActive = (int)UserState.Inactive;
                }
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }

        internal async Task<bool> DeleteUser(int userId)
        {
            // Remove from Friends table.
            var queryFriends = await _dbContext.Friends.Where(f => f.UserId == userId || f.FriendUserId == userId).ToListAsync();
            if (queryFriends.Count() > 0)
            {
                _dbContext.Friends.RemoveRange(queryFriends);
            }

            // Remove from FavoriteMovies table.
            var queryFavoriteMovies = await _dbContext.FavoriteMovies.Where(m => m.UserId == userId).ToListAsync();
            if (queryFavoriteMovies.Count() > 0)
            {
                _dbContext.FavoriteMovies.RemoveRange(queryFavoriteMovies);
            }

            // Remove from FavoriteSeries table.
            var queryFavoriteSeries = await _dbContext.FavoriteSeries.Where(s => s.UserId == userId).ToListAsync();
            if (queryFavoriteSeries.Count() > 0)
            {
                _dbContext.FavoriteSeries.RemoveRange(queryFavoriteSeries);
            }

            // Remove from Messages table.
            var queryMessages = await _dbContext.Messages.Where(x => x.SenderUserId == userId ||x.ReceiverUserId == userId).ToListAsync();
            if (queryMessages.Count() > 0)
            {
                _dbContext.Messages.RemoveRange(queryMessages);
            }

            // Remove from MovieRatings table.
            var queryMovieRatings = await _dbContext.MovieRatings.Where(x => x.UserId == userId).ToListAsync();
            if (queryMovieRatings.Count() > 0)
            {
                _dbContext.MovieRatings.RemoveRange(queryMovieRatings);
            }

            // Remove from SerieRatings table.
            var querySerieRatings = await _dbContext.SerieRatings.Where(x => x.UserId == userId).ToListAsync();
            if (querySerieRatings.Count() > 0)
            {
                _dbContext.SerieRatings.RemoveRange(querySerieRatings);
            }

            // Remove from MovieReviews table.
            var queryMovieReviews = await _dbContext.MovieReviews.Where(x => x.UserId == userId).ToListAsync();
            if (queryMovieReviews.Count() > 0)
            {
                _dbContext.MovieReviews.RemoveRange(queryMovieReviews);
            }

            // Remove from SerieReviews table.
            var querySerieReviews = await _dbContext.SerieReviews.Where(x => x.UserId == userId).ToListAsync();
            if (querySerieReviews.Count() > 0)
            {
                _dbContext.SerieReviews.RemoveRange(querySerieReviews);
            }

            // Remove from News table.
            var queryNews = await _dbContext.News.Where(x => x.UserId == userId).ToListAsync();
            if (queryNews.Count() > 0)
            {
                _dbContext.News.RemoveRange(queryNews);
            }

            // Remove from WatchedMovies table.
            var queryWatchedMovies = await _dbContext.WatchedMovies.Where(x => x.UserId == userId).ToListAsync();
            if (queryWatchedMovies.Count() > 0)
            {
                _dbContext.WatchedMovies.RemoveRange(queryWatchedMovies);
            }

            // Remove from WatchedSeriesSeasonsEpisodes table.
            var queryWatchedSeriesSeasonsEpisodes = await _dbContext.WatchedSeriesSeasonsEpisodes.Where(x => x.UserId == userId).ToListAsync();
            if (queryWatchedSeriesSeasonsEpisodes.Count() > 0)
            {
                _dbContext.WatchedSeriesSeasonsEpisodes.RemoveRange(queryWatchedSeriesSeasonsEpisodes);
            }

            // Remove from Posts table.
            SocialService socialService = new SocialService(_dbContext);
            var queryPosts = await _dbContext.Posts.Where(x => x.UserId == userId).Select(x => x.PostId).ToListAsync();
            if(queryPosts.Count() > 0)
            {
                foreach (var post in queryPosts)
                {
                    await socialService.deletePost(post);
                }
            }

            // Remove from User table.
            var queryUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (queryUser != null)
            {
                _dbContext.Users.Remove(queryUser);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            };
            return false; ;
        }
    }
}

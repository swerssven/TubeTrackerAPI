using MessagePack.Formatters;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
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
    }
}

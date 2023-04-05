using MessagePack.Formatters;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
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

        internal async Task<bool> CheckEmail(string email)
        {
            return await _dbContext.Users.AnyAsync(a => a.Email == email);
        }

        internal async Task<bool> CheckNickname(string nickname)
        {
            return await _dbContext.Users.AnyAsync(a => a.Nickname == nickname);
        }
    }
}

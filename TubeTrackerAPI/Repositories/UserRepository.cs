using Microsoft.EntityFrameworkCore;
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

            var user = await _dbContext.Users.Include(x => x.Posts).AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(username) && x.Password.Equals(password));
                        
            if (user != null)
            {
                result = user.UserId;
            }

            return result;
        }
    }
}

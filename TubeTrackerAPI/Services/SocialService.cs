using Azure.Core;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    public class SocialService
    {
        private readonly TubeTrackerDbContext _dbContext;

        public SocialService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        internal async Task<IEnumerable<Post>> GetPostsList(bool forFriends, int userId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<Post> postsResponse = await socialRepository.GetPostsList(forFriends, userId);

            return postsResponse;
        }

        internal async Task<IEnumerable<Post>> CreatePost(CreatePostRequest request)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<Post> postsResponse = await socialRepository.CreatePost(request);

            return postsResponse;
        }
    }
}

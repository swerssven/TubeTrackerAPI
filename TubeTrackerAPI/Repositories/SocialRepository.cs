using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    public class SocialRepository
    {
        private const string URL = "https://api.themoviedb.org/3";
        private const string apiKey = "7d22105ae1b958ce88fe42db67a97318";
        private readonly TubeTrackerDbContext _dbContext;

        public SocialRepository(TubeTrackerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        internal async Task<IEnumerable<Post>> GetPostsList(bool forFriends, int userId)
        {
            IEnumerable<Post> posts = new List<Post>();

            if (forFriends)
            {
                var friendIdList = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendshipStatus == 1).Select(f => f.FriendUserId).ToListAsync();

                posts = await _dbContext.Posts.Where(p => friendIdList.Contains(p.UserId)).OrderByDescending(p => p.CreationDate).ToListAsync();
            }
            else
            {
                posts = await _dbContext.Posts.Where(p => p.UserId == userId).OrderByDescending(p => p.CreationDate).ToListAsync();
            }
            return posts;
        }

        internal async Task<IEnumerable<Post>> CreatePost(CreatePostRequest request)
        {
            Post post = new Post();
            post.UserId = request.UserId;
            post.Content = request.Content;
            post.CreationDate = DateTime.UtcNow;

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            return await _dbContext.Posts.OrderByDescending(p => p.CreationDate).ToListAsync();
        }
    }
}

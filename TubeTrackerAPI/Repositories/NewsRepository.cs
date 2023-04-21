using Azure.Core;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    internal class NewsRepository
    {
        private readonly TubeTrackerDbContext _dbContext;

        internal NewsRepository(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        internal async Task<IEnumerable<NewsDto>> CreateNewsArticle(News newsArticle)
        {
            _dbContext.News.Add(newsArticle);
            await _dbContext.SaveChangesAsync();

            return await GetNewsArticlesList();
        }

        internal async Task<IEnumerable<NewsDto>> GetNewsArticlesList()
        {
            List<NewsDto> NewsArticleList = new List<NewsDto>();

            NewsArticleList = await _dbContext.News.OrderByDescending(p => p.CreationDate)
                .Select(n => new NewsDto()
                {
                    NewsId = n.NewsId,
                    ContentEs = n.ContentEs,
                    ContentEn = n.ContentEn,
                    creatorNickname = n.User.Nickname,
                    userImage = n.User.Image,
                    CreationDate = n.CreationDate
                }).ToListAsync();

            return NewsArticleList;
        }
    }
}

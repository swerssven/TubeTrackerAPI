using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    internal class NewsService
    {
        private readonly TubeTrackerDbContext _dbContext;

        internal NewsService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        internal async Task<IEnumerable<NewsDto>> CreateNewsArticle(CreateNewsRequest newsRequest)
        {
            NewsRepository newsRepository = new NewsRepository(_dbContext);

            News newsArticle = new News();
            newsArticle.UserId = newsRequest.UserId;
            newsArticle.TitleEs = newsRequest.TitleEs;
            newsArticle.TitleEn = newsRequest.TitleEn;
            newsArticle.ContentEs = newsRequest.ContentEs;
            newsArticle.ContentEn = newsRequest.ContentEn;
            newsArticle.TitleEs = newsRequest.TitleEs;
            newsArticle.CreationDate = DateTime.UtcNow;

            return await newsRepository.CreateNewsArticle(newsArticle);
        }

        internal async Task<IEnumerable<NewsDto>> GetNewsArticlesList()
        {
            NewsRepository newsRepository = new NewsRepository(_dbContext);

            return await newsRepository.GetNewsArticlesList();
        }
    }
}


using Newtonsoft.Json;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.Models.Response;

namespace TubeTrackerAPI.Services
{
    public class MovieService
    {
        public async Task<MovieResponse> GetMovieSearchList(string filter, int page)
        {
            string resultStr = await new MovieRepository().GetMovieSearchList(filter, page);

            MovieResponse movieResponse = JsonConvert.DeserializeObject<MovieResponse>(resultStr);

            return movieResponse;  
        }
    }
}

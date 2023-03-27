using Newtonsoft.Json;
using TubeTrackAPI.Repositories;
using TubeTrackerAPI.Models.Response;

namespace TubeTrackAPI.Services
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

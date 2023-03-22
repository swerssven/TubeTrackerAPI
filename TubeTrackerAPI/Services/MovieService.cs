using Newtonsoft.Json;
using TubeTrackAPI.Models;
using TubeTrackAPI.Repositories;

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

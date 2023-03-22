using TubeTrackAPI.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace TubeTrackAPI.Repositories
{
    public class MovieRepository
    {
        private const string URL = "https://api.themoviedb.org/3";
        private const string apiKey = "7d22105ae1b958ce88fe42db67a97318";

        public async Task<string> GetMovieSearchList(string filter, int page)
        {
            string apiURL = $"/search/movie?api_key={apiKey}&language=en-US&query={filter}&page={page}&include_adult=false";
            List<Movie> movieList = new List<Movie>();

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URL + apiURL))
                {
                    string apiResponse = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }

                    return apiResponse;
                }
            }
        }
    }
}

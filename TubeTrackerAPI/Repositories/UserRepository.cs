namespace TubeTrackerAPI.Repositories
{
    internal class UserRepository
    {
        internal async Task<int> GetUserId(string username, string password)
        {
            int result = 0;

            if (username == "caca")
            {
                result = 1;
            }
            
            return result;
        }
    }
}

namespace TubeTrackerAPI.Repositories
{
    internal class UserRepository
    {
        internal async Task<int> GetUserId(string user, string password)
        {
            int result = 0;

            if (user == "caca")
            {
                result = 1;
            }
            
            return result;
        }
    }
}

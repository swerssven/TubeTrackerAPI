namespace TubeTrackerAPI.Models.Response
{
    internal class GetTokenResponse : BaseResponse
    {
        internal UserToken Token { set; get; }
    }
}

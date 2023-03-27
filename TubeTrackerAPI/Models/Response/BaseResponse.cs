using TubeTrackerAPI.Models.Enum;

namespace TubeTrackerAPI.Models.Response
{
    internal class BaseResponse
    {
        internal StatusEnum Status { get; set; }
        internal string Message { get; set; }

        internal BaseResponse()
        {
            this.Status = StatusEnum.UnkownError;
        }
    }
}

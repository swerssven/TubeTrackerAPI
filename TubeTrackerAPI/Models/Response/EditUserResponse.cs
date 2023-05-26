using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Models.Response
{
    public class EditUserResponse
    {
        internal StatusEnum Status { get; set; }
        internal string Message { get; set; }

        internal UserDto user { get; set; }

        internal EditUserResponse()
        {
            this.Status = StatusEnum.UnkownError;
        }
    }
}

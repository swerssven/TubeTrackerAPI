namespace TubeTrackerAPI.Models.Enum
{
    internal enum StatusEnum
    {
        // Estados Ok
        Ok = 0,

        // Estados de error
        UnkownError = -1,
        NotFound = -2,
        EmailAlreadyExists = -3,
        NickNameAlreadyExists = -4,
        EmailAndNicknameAlreadyExist = -5,
        UserNotExist = -6
    }
}

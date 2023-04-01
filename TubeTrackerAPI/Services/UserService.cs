using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    internal class UserService
    {
        private readonly TubeTrackerDbContext _tubeTrackerDbContext;

        internal UserService(TubeTrackerDbContext tubeTrackerDbContext)
        {
            _tubeTrackerDbContext = tubeTrackerDbContext;
        }
        internal async Task<BaseResponse> CreateUser(User user)
        {
            BaseResponse response = new BaseResponse();

            try
            {
                user.RolId = (int)RolEnum.User;
                user.RegistryDate = DateTime.UtcNow;
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                bool email = await userRepository.CheckEmail(user.Email);
                bool nickname = await userRepository.CheckNickname(user.Nickname);

                if (email)
                {
                    response.Status = StatusEnum.EmailAlreadyExists;
                }
                else if (nickname)
                {
                    response.Status = StatusEnum.NickNameAlreadyExists;
                }
                else if (email && nickname)
                {
                    response.Status = StatusEnum.EmailAndNicknameAlreadyExist;
                }
                else
                {
                    await userRepository.CreateUser(user);
                    response.Status = StatusEnum.Ok;
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusEnum.UnkownError;
                response.Message = ex.ToString();
            }

            return response;
        }

        internal async Task<UserResponse> GetUser(int id)
        {
            BaseResponse response = new BaseResponse();
            UserResponse userResponse = new UserResponse();
            try
            {
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                userResponse = await userRepository.GetUser(id);

                if (userResponse != null)
                {
                    response.Status = StatusEnum.Ok;
                } else
                {
                    response.Status = StatusEnum.UserNotExist;
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusEnum.UnkownError;
                response.Message = ex.ToString();
            }

            return userResponse;
        }
    }
}

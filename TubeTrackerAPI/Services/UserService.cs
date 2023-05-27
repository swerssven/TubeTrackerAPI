using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
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

        internal async Task<UserDto> GetUser(int id)
        {
            BaseResponse response = new BaseResponse();
            UserDto userDto = new UserDto();
            try
            {
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                userDto = await userRepository.GetUser(id);

                if (userDto != null)
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

            return userDto;
        }

        internal async Task<List<usersGridDto>> GetUserList()
        {
            BaseResponse response = new BaseResponse();
            List<usersGridDto> usersGridDto = new List<usersGridDto>();
            try
            {
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                usersGridDto = await userRepository.GetUserList();

                if (usersGridDto != null)
                {
                    response.Status = StatusEnum.Ok;
                }
                else
                {
                    response.Status = StatusEnum.UserNotExist;
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusEnum.UnkownError;
                response.Message = ex.ToString();
            }

            return usersGridDto;
        }

        internal async Task<UserStatisticsDto> GetUserStatistics(int userId)
        {
            BaseResponse response = new BaseResponse();
            UserStatisticsDto userStatisticsDto = new UserStatisticsDto();
            try
            {
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                userStatisticsDto = await userRepository.GetUserStatistics(userId);

                if (userStatisticsDto != null)
                {
                    response.Status = StatusEnum.Ok;
                }
                else
                {
                    response.Status = StatusEnum.UserNotExist;
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusEnum.UnkownError;
                response.Message = ex.ToString();
            }

            return userStatisticsDto;
        }

        internal async Task<EditUserResponse> EditUser(EditUserRequest user)
        {
            EditUserResponse response = new EditUserResponse();

            try
            {
                UserRepository userRepository = new UserRepository(_tubeTrackerDbContext);
                bool userEmail = await userRepository.CheckUserEmail(user.UserId, user.Email);
                bool userNickname = await userRepository.CheckUserNickname(user.UserId, user.Nickname);

                bool email = await userRepository.CheckEmail(user.Email);
                bool nickname = await userRepository.CheckNickname(user.Nickname);

                if (!userEmail && email)
                {
                    response.Status = StatusEnum.EmailAlreadyExists;
                }
                else if (!userNickname && nickname)
                {
                    response.Status = StatusEnum.NickNameAlreadyExists;
                }
                else if (!userEmail && !userNickname && email && nickname)
                {
                    response.Status = StatusEnum.EmailAndNicknameAlreadyExist;
                }
                else
                {
                    response.user = await userRepository.EditUser(user);
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
    }
}

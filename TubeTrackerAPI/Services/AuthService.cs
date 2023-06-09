﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Models.Response;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;

namespace TubeTrackerAPI.Services
{
    internal class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly TubeTrackerDbContext _dbContext;

        public AuthService(IConfiguration configuration, TubeTrackerDbContext dbContext) 
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        internal async Task<GetTokenResponse> GetToken(GetTokenRequest request)
        {
            GetTokenResponse response = new GetTokenResponse();

            try
            {
                UserRepository userRepository = new UserRepository(_dbContext);
                int? userId = await userRepository.GetUserId(request.Email, request.Password);

                response.Status = StatusEnum.NotFound;

                if (userId != null && userId > 0)
                {
                    response.Token = this.Authenticate((int)userId);
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

        private UserToken Authenticate(int userId)
        {
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresIn"));
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:SecurityKey"));
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("userId", userId.ToString())
                }),
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new UserToken
            {
                Token = token,
                ExpiresIn = _configuration.GetValue<int>("Jwt:ExpiresIn") * 60,
                TokenType = "Bearer"
            };
        }
    }
}

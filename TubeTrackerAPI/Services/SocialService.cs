﻿using Azure.Core;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Repositories;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Services
{
    public class SocialService
    {
        private readonly TubeTrackerDbContext _dbContext;

        public SocialService(TubeTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        internal async Task<IEnumerable<FriendDto>> GetSearchFriendsList(int userId, string searchParam)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<FriendDto> searchFriendsResponse = await socialRepository.GetSearchFriendsList(userId, searchParam);

            return searchFriendsResponse;
        }

        internal async Task<IEnumerable<FriendDto>> GetFriendsList(int userId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<FriendDto> friendsResponse = await socialRepository.GetFriendsList(userId);

            return friendsResponse;
        }

        internal async Task<FriendDto> CreateFriendInvitation(int userId, int friendUserId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            FriendDto postsResponse = await socialRepository.CreateFriendInvitation(userId, friendUserId);

            return postsResponse;
        }

        internal async Task<FriendDto> AcceptFriendship(int userId, int friendUserId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            FriendDto postsResponse = await socialRepository.AcceptFriendship(userId, friendUserId);

            return postsResponse;
        }

        internal async Task<IEnumerable<Message>> CreateMessage(CreateMessageRequest request)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<Message> messagesResponse = await socialRepository.CreateMessage(request);

            return messagesResponse;
        }

        internal async Task<IEnumerable<Message>> getMessagesList(int userId, int friendUserId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<Message> messagesResponse = await socialRepository.getMessagesList(userId, friendUserId);

            return messagesResponse;
        }

        internal async Task<IEnumerable<PostDto>> GetPostsList(bool forFriends, int userId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<PostDto> postsResponse = await socialRepository.GetPostsList(forFriends, userId);

            return postsResponse;
        }

        internal async Task<IEnumerable<PostDto>> CreatePost(CreatePostRequest request)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<PostDto> postsResponse = await socialRepository.CreatePost(request);

            return postsResponse;
        }
    }
}
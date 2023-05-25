using Azure.Core;
using Microsoft.EntityFrameworkCore;
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

        internal async Task<IEnumerable<FriendDto>> GetFriendsWithMessagesList(int userId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<FriendDto> friendsResponse = await socialRepository.GetFriendsWithMessagesList(userId);

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

        internal async Task<MessageDto> getMessagesList(int userId, int friendUserId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            MessageDto messagesResponse = await socialRepository.getMessagesList(userId, friendUserId);

            return messagesResponse;
        }

        internal async Task<int> getNumberUnreadMessages(int userId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            int unreadMessages = await socialRepository.getNumberUnreadMessages(userId);

            return unreadMessages;
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

        internal async Task<IEnumerable<PostCommentDto>> CreatePostComment(CreatePostCommentRequest request)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            PostComment postComment = new PostComment();
            postComment.PostId = request.PostId;
            postComment.UserId = request.UserId;
            postComment.Content = request.Content;
            postComment.CreationDate = DateTime.UtcNow;

            IEnumerable<PostCommentDto> postsResponse = await socialRepository.CreatePostComment(postComment);

            return postsResponse;
        }

        internal async Task<IEnumerable<PostCommentDto>> getCommentsList(int postId)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            IEnumerable<PostCommentDto> postCommentsList = await socialRepository.getCommentsList(postId);

            return postCommentsList;
        }

        internal async Task<bool> createPostLike(int userId, int postId, bool liked)
        {
            SocialRepository socialRepository = new SocialRepository(_dbContext);

            bool likedResponse = await socialRepository.createPostLike(userId, postId, liked);

            return likedResponse;
        }
    }
}

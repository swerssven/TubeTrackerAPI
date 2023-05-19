using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.TubeTrackerContext;
using TubeTrackerAPI.TubeTrackerEntities;

namespace TubeTrackerAPI.Repositories
{
    public class SocialRepository
    {
        private const string URL = "https://api.themoviedb.org/3";
        private const string apiKey = "7d22105ae1b958ce88fe42db67a97318";
        private readonly TubeTrackerDbContext _dbContext;

        public SocialRepository(TubeTrackerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        internal async Task<IEnumerable<FriendDto>> GetSearchFriendsList(int userId, string searchParam)
        {
            var friendIdList = await _dbContext.Friends.Where(f => f.UserId == userId).ToListAsync();

            IEnumerable<FriendDto> UserList = await _dbContext.Users.Where(u => u.Nickname.Contains(searchParam) && u.UserId != userId)
                .Select(f => new FriendDto()
                {
                    UserId = f.UserId,
                    FriendNickname = f.Nickname,
                    FriendImage = f.Image,
                    FriendshipStatus = (int)FriendshipStatus.NotFriends
                }).ToListAsync();

            foreach (var user in UserList)
            {
                foreach (var friend in friendIdList)
                {
                    if (user.UserId == friend.FriendUserId)
                    {
                        user.FriendshipStatus = friend.FriendshipStatus;
                    }
                }
            }

            return UserList.OrderBy(f => f.FriendshipStatus);
        }

        internal async Task<IEnumerable<FriendDto>> GetFriendsList(int userId)
        {
            IEnumerable<FriendDto> friendList = await _dbContext.Friends.Where(f => f.UserId == userId)
                .Select(f => new FriendDto()
                {
                    UserId = f.FriendUserId,
                    FriendNickname = f.FriendUser.Nickname,
                    FriendImage = f.FriendUser.Image,
                    FriendshipStatus = f.FriendshipStatus
                }).OrderByDescending(f => f.FriendshipStatus).ToListAsync();

            return friendList;
        }

        internal async Task<IEnumerable<FriendDto>> GetFriendsWithMessagesList(int userId)
        {
            List<int> friendIdList = await _dbContext.Messages.Where(m => m.ReceiverUserId == userId).Select(m => m.SenderUserId).ToListAsync();
            IEnumerable<FriendDto> friendList = await _dbContext.Friends.Where(f => f.UserId == userId && friendIdList.Contains(f.FriendUserId))
                .Select(f => new FriendDto()
                {
                    UserId = f.FriendUserId,
                    FriendNickname = f.FriendUser.Nickname,
                    FriendImage = f.FriendUser.Image,
                    FriendshipStatus = f.FriendshipStatus,
                    NewMessagesCount = _dbContext.Messages.Where(m => m.ReceiverUserId == userId && m.SenderUserId == f.FriendUserId && m.IsRead == false).Count()
                }).ToListAsync();

            return friendList;
        }

        internal async Task<FriendDto> CreateFriendInvitation(int userId, int friendUserId)
        {
            Friend invitation1 = new Friend();
            invitation1.UserId = userId;
            invitation1.FriendUserId = friendUserId;
            invitation1.FriendshipStatus = (int)FriendshipStatus.Invited;

            Friend invitation2 = new Friend();
            invitation2.UserId = friendUserId;
            invitation2.FriendUserId = userId;
            invitation2.FriendshipStatus = (int)FriendshipStatus.Accept;

            _dbContext.Friends.Add(invitation1);
            _dbContext.Friends.Add(invitation2);
            await _dbContext.SaveChangesAsync();

            FriendDto friend = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendUserId == friendUserId)
                .Select(f => new FriendDto()
                {
                    UserId = f.FriendUserId,
                    FriendNickname = f.FriendUser.Nickname,
                    FriendImage = f.FriendUser.Image,
                    FriendshipStatus = f.FriendshipStatus
                }).FirstOrDefaultAsync();

            return friend;
        }

        internal async Task<FriendDto> AcceptFriendship(int userId, int friendUserId)
        {
            var friendsQuery1 = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendUserId == friendUserId).FirstOrDefaultAsync();
            var friendsQuery2 = await _dbContext.Friends.Where(f => f.UserId == friendUserId && f.FriendUserId == userId).FirstOrDefaultAsync();

            friendsQuery1.FriendshipStatus = (int)FriendshipStatus.Friends;
            friendsQuery2.FriendshipStatus = (int)FriendshipStatus.Friends;

            _dbContext.Friends.Update(friendsQuery1);
            _dbContext.Friends.Update(friendsQuery2);
            await _dbContext.SaveChangesAsync();

            FriendDto friend = await _dbContext.Friends.Where(f => f.UserId == friendUserId && f.FriendUserId == userId)
                .Select(f => new FriendDto()
                {
                    UserId = f.User.UserId,
                    FriendNickname = f.User.Nickname,
                    FriendImage = f.User.Image,
                    FriendshipStatus = f.FriendshipStatus
                }).FirstOrDefaultAsync();

            return friend;
        }

        internal async Task<IEnumerable<Message>> CreateMessage(CreateMessageRequest request)
        {
            await _dbContext.Messages.Where(m => m.ReceiverUserId == request.SenderUserId && m.SenderUserId == request.ReceiverUserId)
                .ForEachAsync(m => { m.IsRead = true; }); // Mark old incomming messages as read.

            Message message = new Message();
            message.SenderUserId = request.SenderUserId;
            message.ReceiverUserId = request.ReceiverUserId;
            message.Content = request.Content;
            message.CreationDate = DateTime.UtcNow;
            message.IsRead = false;

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            IEnumerable<Message> messagesResponse = await _dbContext.Messages.Where(m => m.SenderUserId == request.SenderUserId || m.ReceiverUserId == request.SenderUserId).OrderBy(m => m.CreationDate).ToListAsync();

            return messagesResponse;
        }

        internal async Task<MessageDto> getMessagesList(int userId, int friendUserId)
        {
            var messagesQuery = await _dbContext.Messages.Where(m => m.ReceiverUserId == userId && m.SenderUserId == friendUserId).ToListAsync();
            messagesQuery.ForEach(m => { m.IsRead = true; }); // Mark old incomming messages as read.
            _dbContext.UpdateRange(messagesQuery);
            await _dbContext.SaveChangesAsync();

            IEnumerable<Message> messagesList = await _dbContext.Messages
                .Where(m => m.SenderUserId == userId && m.ReceiverUserId == friendUserId || m.SenderUserId == friendUserId && m.ReceiverUserId == userId)
                .OrderBy(m => m.CreationDate).ToListAsync();

            MessageDto response = await _dbContext.Users.Where(u => u.UserId == friendUserId).Select(m => new MessageDto()
            {
                MessagesList = messagesList,
                ReceiverImage = m.Image,
                ReceiverName = m.Nickname
            }).FirstOrDefaultAsync();

            return response;
        }

        //internal async Task<int> getNumberUnreadMessages() 

        internal async Task<IEnumerable<PostDto>> GetPostsList(bool forFriends, int userId)
        {
            IEnumerable<PostDto> posts = new List<PostDto>();

            if (forFriends)
            {
                var friendIdList = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendshipStatus == 1).Select(f => f.FriendUserId).ToListAsync();

                posts = await _dbContext.Posts.Where(p => p.UserId == userId || friendIdList.Contains(p.UserId)).OrderByDescending(p => p.CreationDate)
                    .Select(p => new PostDto()
                    {
                        PostId = p.PostId,
                        Content = p.Content,
                        UserId = p.UserId,
                        UserNickname = p.User.Nickname,
                        UserImage = p.User.Image,
                        CreationDate = p.CreationDate,
                        PostComments = p.PostComments
                    }).ToListAsync();
            }
            else
            {
                posts = await _dbContext.Posts.Where(p => p.UserId == userId).OrderByDescending(p => p.CreationDate)
                    .Select(p => new PostDto()
                    {
                        PostId = p.PostId,
                        Content = p.Content,
                        UserId = p.UserId,
                        UserNickname = p.User.Nickname,
                        UserImage = p.User.Image,
                        CreationDate = p.CreationDate,
                        PostComments = p.PostComments
                    }).ToListAsync();
            }
            return posts;
        }

        internal async Task<IEnumerable<PostDto>> CreatePost(CreatePostRequest request)
        {
            Post post = new Post();
            post.UserId = request.UserId;
            post.Content = request.Content;
            post.CreationDate = DateTime.UtcNow;

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            List<PostDto> postList = await _dbContext.Posts.Where(p => p.UserId == request.UserId).OrderByDescending(p => p.CreationDate)
                .Select(p => new PostDto()
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    UserId = p.UserId,
                    UserNickname = p.User.Nickname,
                    UserImage = p.User.Image,
                    CreationDate = p.CreationDate,
                    PostComments = p.PostComments
                }).ToListAsync();

            return postList;
        }

        internal async Task<IEnumerable<PostCommentDto>> CreatePostComment(PostComment postComment)
        {
            _dbContext.PostComments.Add(postComment);
            await _dbContext.SaveChangesAsync();

            List<PostCommentDto> postCommentsList = await _dbContext.PostComments.Where(p => p.PostId == postComment.PostId).OrderBy(p => p.CreationDate)
                .Select(p => new PostCommentDto()
                {
                    PostId = postComment.PostId,
                    Content = p.Content,
                    UserId = p.UserId,
                    UserNickname = p.User.Nickname,
                    UserImage = p.User.Image,
                    CreationDate = p.CreationDate
                }).ToListAsync();

            return postCommentsList;
        }

        internal async Task<IEnumerable<PostCommentDto>> getCommentsList(int postId)
        {
            List<PostCommentDto> postCommentsList = await _dbContext.PostComments.Where(p => p.PostId == postId).OrderBy(p => p.CreationDate)
                .Select(p => new PostCommentDto()
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    UserId = p.UserId,
                    UserNickname = p.User.Nickname,
                    UserImage = p.User.Image,
                    CreationDate = p.CreationDate
                }).ToListAsync();

            return postCommentsList;
        }
    }
}

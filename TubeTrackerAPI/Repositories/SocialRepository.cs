﻿using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TubeTrackerAPI.Models;
using TubeTrackerAPI.Models.Enum;
using TubeTrackerAPI.Models.Request;
using TubeTrackerAPI.Services;
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

        internal async Task<IEnumerable<FriendDto>> GetFriendsList(int userId, bool suggestions)
        {
            IEnumerable<FriendDto> friendList = new List<FriendDto>();
            if (!suggestions)
            {
                friendList = await _dbContext.Friends.Where(f => f.UserId == userId)
                    .Select(f => new FriendDto()
                    {
                        UserId = f.FriendUserId,
                        FriendNickname = f.FriendUser.Nickname,
                        FriendImage = f.FriendUser.Image,
                        FriendshipStatus = f.FriendshipStatus
                    }).OrderByDescending(f => f.FriendshipStatus).ToListAsync();
            }
            else
            {
                friendList = await _dbContext.Users
    .Where(user => user.UserId != userId && !_dbContext.Friends.Any(friend => friend.UserId == userId && friend.FriendUserId == user.UserId))
                    .Select(f => new FriendDto()
                    {
                        UserId = f.UserId,
                        FriendNickname = f.Nickname,
                        FriendImage = f.Image,
                        FriendshipStatus = 2
                    }).ToListAsync();
            }

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

            SocialService socialService = new SocialService(_dbContext);
            var queryUser = await _dbContext.Users.Where(u => u.UserId == userId).Select(x => x.Nickname).FirstOrDefaultAsync();
            var queryFriendLanguage = await _dbContext.Users.Where(u => u.UserId == friendUserId).Select(x => x.Language).FirstOrDefaultAsync();

            if(queryFriendLanguage != null && queryFriendLanguage == "es-ES" && friendUserId != 26) //except tube tracker id
            {
                string messageES = "<div><p>¡Alerta de nuevo amigo en Tube Tracker! <b>{{friend}}</b> te ha invitado para sumergirte en aventuras televisivas compartidas. Ve a la sección de <a href=\"/social/find-friends\">Amigos</a> y ¡acepta ahora!</p></div>";
                messageES = messageES.Replace("{{friend}}", queryUser);
                await socialService.CreateMessage(new CreateMessageRequest()
                {
                    SenderUserId = 26, //tube tracker id
                    ReceiverUserId = friendUserId,
                    Content = messageES
                });
            }
            else if(queryFriendLanguage != null && queryFriendLanguage == "en-EN" && friendUserId != 26) //except tube tracker id
            {
                string messageES = "<div> <p>New friend alert on Tube Tracker! <b>{{friend}}</b> invited you to embrace the connection and dive into shared TV adventures. Go to the <a href=\"/social/find-friends\">Friends</a> section and accept now!</p> </div>";
                messageES = messageES.Replace("{{friend}}", queryUser);
                await socialService.CreateMessage(new CreateMessageRequest()
                {
                    SenderUserId = 26, //tube tracker id
                    ReceiverUserId = friendUserId,
                    Content = messageES
                });
            }

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

        internal async Task<FriendDto> AcceptFriendship(int userId, int friendUserId, bool accept)
        {
            var friendsQuery1 = await _dbContext.Friends.Where(f => f.UserId == userId && f.FriendUserId == friendUserId).FirstOrDefaultAsync();
            var friendsQuery2 = await _dbContext.Friends.Where(f => f.UserId == friendUserId && f.FriendUserId == userId).FirstOrDefaultAsync();
            if (accept)
            {
                friendsQuery1.FriendshipStatus = (int)FriendshipStatus.Friends;
                friendsQuery2.FriendshipStatus = (int)FriendshipStatus.Friends;

                _dbContext.Friends.Update(friendsQuery1);
                _dbContext.Friends.Update(friendsQuery2);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.Friends.Remove(friendsQuery1);
                _dbContext.Friends.Remove(friendsQuery2);
                await _dbContext.SaveChangesAsync();
            }

            FriendDto friend = new FriendDto();
            if (accept)
            {
                friend = await _dbContext.Friends.Where(f => f.UserId == friendUserId && f.FriendUserId == userId)
                    .Select(f => new FriendDto()
                    {
                        UserId = f.User.UserId,
                        FriendNickname = f.User.Nickname,
                        FriendImage = f.User.Image,
                        FriendshipStatus = f.FriendshipStatus
                    }).FirstOrDefaultAsync();
            }
            else
            {
                friend = await _dbContext.Users.Where(f => f.UserId == friendUserId)
                    .Select(f => new FriendDto()
                    {
                        UserId = f.UserId,
                        FriendNickname = f.Nickname,
                        FriendImage = f.Image,
                        FriendshipStatus = (int)FriendshipStatus.Declined
                    }).FirstOrDefaultAsync();
            }

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

        internal async Task<int> getNumberUnreadMessages(int userId)
        {
            int unreadMessages = await _dbContext.Messages
                .Where(m => m.ReceiverUserId == userId && m.IsRead == false)
                .CountAsync();

            return unreadMessages;
        }

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
                        LikesCount = p.Users.Count(), // Entity Framework changed name of db table references likes many to many.
                        LikedByUser = !p.Users.Where(l => l.UserId == userId).IsNullOrEmpty()
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
                        LikesCount = p.Users.Count(), // Entity Framework changed name of db table references likes many to many.
                        LikedByUser = !p.Users.Where(l => l.UserId == userId).IsNullOrEmpty()
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
                    PostCommnentsId = p.PostCommnentsId,
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
                    PostCommnentsId = p.PostCommnentsId,
                    PostId = p.PostId,
                    Content = p.Content,
                    UserId = p.UserId,
                    UserNickname = p.User.Nickname,
                    UserImage = p.User.Image,
                    CreationDate = p.CreationDate
                }).ToListAsync();

            return postCommentsList;
        }

        internal async Task<bool> createPostLike(int userId, int postId, bool liked)
        {
            var query = await _dbContext.Users.Include(x => x.PostsNavigation).FirstOrDefaultAsync(u => u.UserId == userId);
            var postQuery = await _dbContext.Posts.Include(x => x.Users).FirstOrDefaultAsync(p => p.PostId == postId);
            bool likedResponse = false;

            if (liked)
            {
                query.PostsNavigation.Add(postQuery);
                await _dbContext.SaveChangesAsync();
                likedResponse = true;
            }
            else
            {
                query.PostsNavigation.Remove(postQuery);
                await _dbContext.SaveChangesAsync();
                likedResponse = false;
            }

            return likedResponse;
        }

        internal async Task<bool> deletePost(int postId)
        {
            var queryPostLikes = await _dbContext.Posts.Include(x => x.Users).FirstOrDefaultAsync(p => p.PostId == postId);
            if (queryPostLikes != null)
            {
                queryPostLikes.Users.Clear();
            }

            var queryPostComments = await _dbContext.PostComments.Where(p => p.PostId == postId).ToListAsync();
            if(queryPostComments.Count() > 0)
            {
                _dbContext.PostComments.RemoveRange(queryPostComments);
            }

            var queryPosts = await _dbContext.Posts.Where(p => p.PostId == postId).FirstOrDefaultAsync();
            if (queryPosts != null)
            {
                _dbContext.Remove(queryPosts);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            };
            return false; ;
        }

        internal async Task<bool> deletePostComment(int postCommnentsId)
        {
            var queryPostComments = await _dbContext.PostComments.Where(p => p.PostCommnentsId == postCommnentsId).ToListAsync();
            if (queryPostComments.Count() > 0)
            {
                _dbContext.RemoveRange(queryPostComments);
            }

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return true;
            };
            return false; ;
        }
    }
}

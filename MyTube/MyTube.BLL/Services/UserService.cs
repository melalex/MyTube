using MyTube.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.BLL.BusinessEntities;
using MyTube.DAL.Interfaces;
using MyTube.DAL.FileStorage.Interfaces;
using MyTube.DAL.Identity.Interfaces;
using MyTube.DAL.Extensions;
using AutoMapper;
using MyTube.DAL.Entities;
using MyTube.BLL.DTO;
using MyTube.BLL.Infrastructure;

namespace MyTube.BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork dataStrore;
        private IStorageFacade fileStore;
        private IIdentityUnitOfWork identityStore;
        private ExecutorSelector _execution = null;

        private ExecutorSelector Execution
        {
            get
            {
                if (_execution == null)
                {
                    _execution = new ExecutorSelector(identityStore);
                }
                return _execution;
            }
        }

        public UserService(IUnitOfWork dataStrore, IStorageFacade fileStore, IIdentityUnitOfWork identityStore)
        {
            this.dataStrore = dataStrore;
            this.fileStore = fileStore;
            this.identityStore = identityStore;
        }

        public void SetStorageFolder(string storageFolder)
        {
            fileStore.SetStorageFolder(storageFolder);
        }

        #region ChannelLogic
        public async Task<string> CreateChannelAsync(string userName)
        {
            Channel channel = new Channel
            {
                Username = userName,
                AvatarUri = fileStore.DefaultAvatarUri,
            };
            return await dataStrore.Channels.CreateAsync(channel);
        }

        public async Task<ChannelProxy> GetChannelAsync(string id)
        {
            ChannelProxy result = null;
            Channel channel = await dataStrore.Channels.Get(id);
            if (channel != null)
            {
                result = new ChannelProxy(dataStrore, channel);
            }
            return result;
        }

        public async Task EditChannelUsernameAsync(string channelId, string username)
        {
            await dataStrore.Channels.UpdateUsernameAsync(channelId, username);         
        }

        public async Task EditChannelAvatarAsync(string channelId, string avatarPath)
        {
            Channel channel = await dataStrore.Channels.Get(channelId);
            if (channel.AvatarUri != fileStore.DefaultAvatarUri)
            {
                fileStore.DeleteAvatar(channel.AvatarUri);
            }
            string avatarUri = await fileStore.SaveAvatar(avatarPath);
            await dataStrore.Channels.UpdateAvatarAsync(channelId, avatarUri);
        }
        #endregion

        #region VideoLogic
        public async Task<string> CreateVideoAsync(
            string uploderId,
            string name,
            string description,
            string category,
            string[] tags,
            string videoPath,
            string posterPath = null
            )
        {
            string videoUri = fileStore.SaveVideoAsync(videoPath);
            string posterUri = null;
            if (posterPath != null)
            {
                posterUri = await fileStore.SavePoster(posterPath);
            }
            else
            {
                posterUri = fileStore.DefaultPosterUriAsync(videoPath);
            }
            Video newVideo = new Video
            {
                Name = name,
                Description = description,
                Category = category,
                Tags = new List<string>(tags),
                VideoUrl = videoUri,
                PosterUrl = posterUri,
                Likes = 0,
                Dislikes = 0,
                Views = 0,
                UploadDate = DateTimeOffset.Now,
                UploderIdString = uploderId, 
            };
            string createdVideoId = await dataStrore.Videos.CreateAsync(newVideo);
            return createdVideoId;
        }

        public async Task<VideoProxy> GetVideoAsync(string id)
        {
            VideoProxy result = null;
            Video video = await dataStrore.Videos.Get(id);
            if (video != null)
            {
                result = await VideoProxy.Create(dataStrore, video);
            }
            return result;
        }

        public async Task<IEnumerable<VideoProxy>> GetSimilarVideosAsync(VideoProxy video, int skip, int limit)
        {
            IEnumerable<Video> similarVideos = await dataStrore.Videos.SearchBYTagsAsync(video.Tags, skip, limit);
            var tasks = similarVideos.Where(v => v.IdString != video.Id)
                .Select(async v => await VideoProxy.Create(dataStrore, v));
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<VideoProxy>> GetPopularVideosAsync(int skip, int limit)
        {
            var result = await dataStrore.Videos.GetPopularVideosAsync(skip, limit);
            var tasks = result.Select(async v => await VideoProxy.Create(dataStrore, v));
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(string videoId, int skip, int limit)
        {
            IEnumerable<Comment> comments = await dataStrore.Comments.GetCommentsFromVideoAsync(videoId, skip, limit);

            var tasks = comments.Select(async x => {
                Channel commentator = await dataStrore.Channels.Get(x.ComentatorIdString);
                return new CommentDTO
                {
                    Id = x.IdString,
                    VideoId = videoId,
                    CommentatorAvatarUri = commentator.AvatarUri,
                    CommentatorUsername = commentator.Username,
                    CommentatorId = commentator.IdString,
                    CommentDateTime = x.CommentDateTime,
                    CommentText = x.Text,
                };
            }).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task<long> GetCommentsCountAsync(string videoId)
        {
            return await dataStrore.Comments.CommentsCountFromVideoAsync(videoId);
        }

        public async void AddCommentAsync(CommentDTO comment)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<CommentDTO, Comment>()
                        .ForMember(s => s.ComentatorIdString, s => s.MapFrom(scr => scr.CommentatorId))
                        .ForMember(s => s.DestinationVideoIdString, s => s.MapFrom(scr => scr.VideoId))
                        .ForMember(s => s.CommentDateTime, s => s.MapFrom(scr => scr.CommentDateTime))
                        .ForMember(s => s.Text, s => s.MapFrom(scr => scr.CommentText))
                        );
            await dataStrore.Comments.CreateAsync(Mapper.Map<CommentDTO, Comment>(comment));
        }

        public async void DeleteVideoAsync(VideoProxy video)
        {
            fileStore.DeletePoster(video.PosterUri);
            fileStore.DeleteVideo(video.VideoUri);
            await dataStrore.Comments.DeleteCommentsFromVideoAsync(video.Id);
        }

        public async void EstimateVideoAsync(Video video, ViewedVideoTransferDTO transfer)
        {
            ViewedVideoTransfer existingTransfer = await dataStrore.ViewedVideoTransfers.GetByChannelVideoAsync(
                transfer.Viewer, transfer.ViewedVideo
                );

            if (existingTransfer != null)
            {
                Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransferDTO, ViewedVideoTransfer>()
                            .ForMember(s => s.ViewerIdString, s => s.MapFrom(scr => scr.Viewer))
                            .ForMember(s => s.ViewedVideoIdString, s => s.MapFrom(scr => scr.ViewedVideo))
                            .ForMember(s => s.Status, s => s.MapFrom(scr => (DAL.Entities.ViewStatus)scr.Status))
                            .ForMember(s => s.ShowDateTime, s => s.MapFrom(scr => scr.ShowDateTime))
                            );
                await dataStrore.ViewedVideoTransfers.CreateAsync(
                    Mapper.Map<ViewedVideoTransferDTO, ViewedVideoTransfer>(transfer)
                    );
            }
            else
            {
                switch (existingTransfer.Status)
                {
                    case DAL.Entities.ViewStatus.DISLIKE:
                        video.Dislikes--;
                        video.Views--;
                        break;
                    case DAL.Entities.ViewStatus.LIKE:
                        video.Likes--;
                        video.Views--;
                        break;
                    default:
                        video.Views--;
                        break;
                }
            }

            switch (transfer.Status)
            {
                case Interfaces.ViewStatus.DISLIKE:
                    video.Dislikes++;
                    video.Views++;
                    break;
                case Interfaces.ViewStatus.LIKE:
                    video.Likes++;
                    video.Views++;
                    break;
                default:
                    video.Views++;
                    break;
            }
            await dataStrore.Videos.UpdateAsync(video);
        }

        public async Task<ViewedVideoTransferDTO> GetVideoEstimationAsync(string channel, string video)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransfer, ViewedVideoTransferDTO>()
                        .ForMember(s => s.Viewer, s => s.MapFrom(scr => scr.ViewerIdString))
                        .ForMember(s => s.ViewedVideo, s => s.MapFrom(scr => scr.ViewerIdString))
                        .ForMember(s => s.Status, s => s.MapFrom(scr => (Interfaces.ViewStatus)scr.Status))
                        .ForMember(s => s.ShowDateTime, s => s.MapFrom(scr => scr.ShowDateTime))
                        );
            ViewedVideoTransfer existingTransfer = await dataStrore.ViewedVideoTransfers.GetByChannelVideoAsync(channel, video);
            return Mapper.Map<ViewedVideoTransfer, ViewedVideoTransferDTO>(existingTransfer);
        }
        #endregion

        #region SubscribtionLogic
        public async Task SubscribeAsync(SubscriptionDTO subscription)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<SubscriptionDTO, Subscription>()
                        .ForMember(s => s.PublisherIdString, s => s.MapFrom(scr => scr.Publisher))
                        .ForMember(s => s.SubscriberIdString, s => s.MapFrom(scr => scr.Subscriber))
                        .ForMember(s => s.StartDate, s => s.MapFrom(scr => scr.StartDate)));
            if (!await dataStrore.Subscriptions.IsSubscriberAsync(subscription.Publisher, subscription.Subscriber)
                && subscription.Publisher != subscription.Subscriber)
            {
                await dataStrore.Subscriptions.CreateAsync(Mapper.Map<SubscriptionDTO, Subscription>(subscription));
            }
        }

        public async Task UnsubscribeAsync(string subscriptionId)
        {
            await dataStrore.Subscriptions.DeleteAsync(subscriptionId);
        }
        #endregion

        #region ReportLogic
        public async Task ReportAsync(string link, string message)
        {
            string executorId = await Execution.selectExecutorAsync();
            Notification reportNotification = new Notification
            {
                DestinationChannelIdString = executorId,
                Link = link,
                NotificationDateTime = DateTimeOffset.Now,
                Text = "Report"
            };
            await dataStrore.Notifications.CreateAsync(reportNotification);
        }
        #endregion
    }
}

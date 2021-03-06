﻿using MyTube.BLL.Interfaces;
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
                result = new ChannelProxy(channel);
            }
            return result;
        }

        public async Task DeleteChannelAsync(string id)
        {
            await dataStrore.Channels.DeleteAsync(id);
        }
        
        public async Task EditChannelUsernameAsync(ChannelProxy channel, string username)
        {
            await dataStrore.Channels.UpdateUsernameAsync(channel.Id, username);
            channel.Username = username;         
        }

        public async Task EditChannelAvatarAsync(ChannelProxy channel, string avatarPath)
        {
            if (channel.AvatarUri != fileStore.DefaultAvatarUri)
            {
                fileStore.DeleteAvatar(channel.AvatarUri);
            }
            string avatarUri = await fileStore.SaveAvatar(avatarPath);
            await dataStrore.Channels.UpdateAvatarAsync(channel.Id, avatarUri);
            channel.AvatarUri = avatarUri;
        }
        #endregion

        #region SearchLogic
        public async Task<IEnumerable<VideoProxy>> FulltextSearchAsync(string queryString, int skip, int limit)
        {
            var videos = await dataStrore.Videos.SearchByStringAsync(queryString, skip, limit);
            var tasks = videos.Select(async x => await VideoProxy.Create(dataStrore, x)).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<VideoProxy>> CategorySearchAsync(string category, int skip, int limit)
        {
            var videos = await dataStrore.Videos.SearchByCategoryAsync(category, skip, limit);
            var tasks = videos.Select(async x => await VideoProxy.Create(dataStrore, x)).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<VideoProxy>> TagsSearchAsync(string tag, int skip, int limit)
        {
            var videos = await dataStrore.Videos.SearchByTagsAsync(new List<string> { tag }, skip, limit);
            var tasks = videos.Select(async x => await VideoProxy.Create(dataStrore, x)).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task<long> FulltextCountSearchAsync(string queryString)
        {
            return await dataStrore.Videos.FulltextCountSearchAsync(queryString);
        }

        public async Task<long> CategorySearchCountAsync(string category)
        {
            return await dataStrore.Videos.CategorySearchCountAsync(category);
        }

        public async Task<long> TagsSearchCountAsync(string tag)
        {
            return await dataStrore.Videos.TagsSearchCountAsync(new List<string> { tag });
        }
        #endregion

        #region VideoLogic
        public async Task<long> VideosCountAsync()
        {
            return await dataStrore.Videos.VideosCountAsync();
        }

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

        public async Task<IEnumerable<VideoProxy>> GetVideosFromChannelAsync(string channel, int skip, int limit)
        {
            var videos = await dataStrore.Videos.GetVideosFromChannelAsync(channel, skip, limit);
            var tasks = videos.Select(async x => await VideoProxy.Create(dataStrore, x)).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task ForEachVideoFromChannelAsync(string channel, Func<VideoProxy, Task> job)
        {
            await dataStrore.Videos.ForEachVideoFromChannelAsync(
                channel, async v => 
                {
                    VideoProxy video = new VideoProxy(v);
                    await job(video);
                });
        }

        public async Task<long> GetVideosFromChannelCountAsync(string channel)
        {
            return await dataStrore.Videos.GetVideosFromChannelCountAsync(channel);
        }

        public async Task<IEnumerable<VideoProxy>> GetSimilarVideosAsync(VideoProxy video, int skip, int limit)
        {
            IEnumerable<Video> similarVideos = await dataStrore.Videos.SearchByTagsAsync(video.Tags, skip, limit);
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

        public async Task AddCommentAsync(CommentDTO comment)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<CommentDTO, Comment>()
                        .ForMember(s => s.Id, s => s.Ignore())
                        .ForMember(s => s.IdString, s => s.MapFrom(scr => scr.Id))
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
            await dataStrore.ViewedVideoTransfers.DeleteHistory(video.Id);
            await dataStrore.ViewingHistories.DeleteHistory(video.Id);
            await dataStrore.Comments.DeleteCommentsFromVideoAsync(video.Id);
            await dataStrore.Videos.DeleteAsync(video.Id);
        }

        public async Task<bool> AddView(VideoProxy video, string userHostAddress)
        {
            if (userHostAddress != null)
            {
                string id = video.Id; 
                bool isWatched = await dataStrore.ViewingHistories.IsWatched(userHostAddress, id);
                if (!isWatched)
                {
                    await dataStrore.Videos.AddView(id);
                    video.Views++;
                    ViewingHistory item = new ViewingHistory
                    {
                        UserHostAddress = userHostAddress,
                        DestinationVideoIdString = id,
                    };
                    await dataStrore.ViewingHistories.CreateAsync(item);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> EstimateVideoAsync(ViewedVideoTransferDTO transfer, VideoProxy video)
        {
            if (transfer.Status == Interfaces.ViewStatus.IGNORE)
            {
                return false;
            }

            ViewedVideoTransfer existingTransfer = await dataStrore.ViewedVideoTransfers.GetByChannelVideoAsync(
                transfer.Viewer, transfer.ViewedVideo
                );

            Interfaces.ViewStatus existingViewStatus = Interfaces.ViewStatus.IGNORE;

            Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransferDTO, ViewedVideoTransfer>()
                            .ForMember(s => s.Id, s => s.Ignore())
                            .ForMember(s => s.ViewedVideo, s => s.Ignore())
                            .ForMember(s => s.Viewer, s => s.Ignore())
                            .ForMember(s => s.IdString, s => s.MapFrom(scr => scr.Id))
                            .ForMember(s => s.ViewerIdString, s => s.MapFrom(scr => scr.Viewer))
                            .ForMember(s => s.ViewedVideoIdString, s => s.MapFrom(scr => scr.ViewedVideo))
                            .ForMember(s => s.Status, s => s.MapFrom(scr => (DAL.Entities.ViewStatus)scr.Status))
                            .ForMember(s => s.ShowDateTime, s => s.MapFrom(scr => scr.ShowDateTime))
                            );

            if (existingTransfer == null)
            {
                await dataStrore.ViewedVideoTransfers.CreateAsync(
                    Mapper.Map<ViewedVideoTransferDTO, ViewedVideoTransfer>(transfer)
                    );
            }
            else
            {
                await dataStrore.ViewedVideoTransfers.UpdateAsync(
                    Mapper.Map<ViewedVideoTransferDTO, ViewedVideoTransfer>(transfer)
                    );
                existingViewStatus = (Interfaces.ViewStatus)existingTransfer.Status;
            }
            
            if (existingViewStatus == Interfaces.ViewStatus.LIKE && transfer.Status == Interfaces.ViewStatus.DISLIKE)
            {
                dataStrore.Videos.RemoveLikeAndAddDislike(transfer.ViewedVideo);
                video.Likes--;
                video.Dislikes++;
            }
            else if (existingViewStatus == Interfaces.ViewStatus.DISLIKE && transfer.Status == Interfaces.ViewStatus.LIKE)
            {
                dataStrore.Videos.RemoveDislikeAndAddLike(transfer.ViewedVideo);
                video.Likes++;
                video.Dislikes--;
            }
            else if (existingViewStatus == Interfaces.ViewStatus.IGNORE && transfer.Status == Interfaces.ViewStatus.LIKE)
            {
                dataStrore.Videos.AddLike(transfer.ViewedVideo);
                video.Likes++;
            }
            else if (existingViewStatus == Interfaces.ViewStatus.IGNORE && transfer.Status == Interfaces.ViewStatus.DISLIKE)
            {
                dataStrore.Videos.AddDislike(transfer.ViewedVideo);
                video.Dislikes++;
            }
            else
            {
                return false;
            }

            return true;
        }

        public async Task<ViewedVideoTransferDTO> GetVideoEstimationAsync(string channel, string video)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransfer, ViewedVideoTransferDTO>()
                        .ForMember(s => s.Id, s => s.MapFrom(scr => scr.IdString))
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
                        .ForMember(s => s.Id, s => s.Ignore())
                        .ForMember(s => s.Publisher, s => s.Ignore())
                        .ForMember(s => s.Subscriber, s => s.Ignore())
                        .ForMember(s => s.IdString, s => s.MapFrom(scr => scr.Id))
                        .ForMember(s => s.PublisherIdString, s => s.MapFrom(scr => scr.Publisher))
                        .ForMember(s => s.SubscriberIdString, s => s.MapFrom(scr => scr.Subscriber))
                        .ForMember(s => s.StartDate, s => s.MapFrom(scr => scr.StartDate)));
            if (!await dataStrore.Subscriptions.IsSubscriberAsync(subscription.Publisher, subscription.Subscriber)
                && subscription.Publisher != subscription.Subscriber)
            {
                await dataStrore.Subscriptions.CreateAsync(Mapper.Map<SubscriptionDTO, Subscription>(subscription));
            }
        }

        public async Task UnsubscribeAsync(string publisher, string subscriber)
        {
            await dataStrore.Subscriptions.UnSubscribeAsync(publisher, subscriber);
        }

        public async Task<long> SubscribersCountAsync(string channel)
        {
            return await dataStrore.Subscriptions.GetSubscribersCountAsync(channel);
        }

        public async Task<bool> IsSubscriberAsync(string publisher, string subscriber)
        {
            return await dataStrore.Subscriptions.IsSubscriberAsync(publisher, subscriber);
        }

        public async Task<IEnumerable<ChannelProxy>> SubscriptionsAsync(string channel, int skip, int limit)
        {
            var subscriptions = await dataStrore.Subscriptions.GetSubscribtionsAsync(channel, skip, limit);
            var tasks = subscriptions.Select(async x => 
            {
                return await GetChannelAsync(x.PublisherIdString);
            }).ToList();
            return await Task.WhenAll(tasks);
        }

        public async Task<long> SubscriptionsCountAsync(string channel)
        {
            return await dataStrore.Subscriptions.GetSubscribtionsCountAsync(channel);
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

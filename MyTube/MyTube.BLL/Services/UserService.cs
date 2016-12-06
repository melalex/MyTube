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

namespace MyTube.BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork dataStrore;
        private IStorageFacade fileStore;
        private IIdentityUnitOfWork identityStore;

        public UserService(IUnitOfWork dataStrore, IStorageFacade fileStore, IIdentityUnitOfWork identityStore)
        {
            this.dataStrore = dataStrore;
            this.fileStore = fileStore;
            this.identityStore = identityStore;

            Mapper.Initialize(cfg => cfg.CreateMap<ChannelProxy, DAL.Entities.Channel>()
            .ForMember("IDString", opt => opt.MapFrom(c => c.Id)));
        }

        #region ChannelLogic
        public async Task<string> CreateChannel(string userName, byte[] avatar = null)
        {
            string avatarUri = null;
            if (avatar != null)
            {
                string name = Guid.NewGuid().ToString();
                avatarUri = fileStore.SaveAvatar(avatar, name);
            }
            else
            {
                avatarUri = fileStore.DefaultAvatarUri;
            }
            DAL.Entities.Channel channel = new DAL.Entities.Channel
            {
                Username = userName,
                AvatarUri = avatarUri,
            };
            return await dataStrore.Channels.CreateAsync(channel);
        }

        public async Task<ChannelProxy> GetChannel(string id)
        {
            DAL.Entities.Channel channel = await dataStrore.Channels.Get(id);
            return new ChannelProxy(dataStrore, channel);
        }

        public async Task EditChannel(ChannelProxy channel, string userName, byte[] avatar = null)
        {
            if (channel.AvatarUri != fileStore.DefaultAvatarUri)
            {
                fileStore.DeleteAvatar(channel.AvatarUri);
            }
            if (avatar != null)
            {
                string name = Guid.NewGuid().ToString();
                channel.AvatarUri = fileStore.SaveAvatar(avatar, name);
            }
            channel.Username = userName;
            await dataStrore.Channels.UpdateAsync(channel.channel);         
        }
        #endregion

        #region VideoLogic
        public async Task<string> CreateVideo(
            string uploderId, string name, string description, string category, List<string> tags, byte[] video, byte[] poster
            )
        {
            string fileName = Guid.NewGuid().ToString();
            string videoUri = fileStore.SaveVideo(video, name);
            string posterUri = null;
            if (poster != null)
            {
                posterUri = fileStore.SavePoster(poster, name);
            }
            else
            {
                posterUri = fileStore.DefaultPosterUri;
            }
            Video newVideo = new DAL.Entities.Video
            {
                Name = name,
                Description = description,
                Category = category,
                Tags = tags,
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

        public async Task<VideoProxy> GetVideo(string id)
        {
            Video video = await dataStrore.Videos.Get(id);
            return await VideoProxy.Create(dataStrore, video);
        }

        public async Task<IEnumerable<VideoProxy>> GetSimilarVideos(VideoProxy video, int skip, int limit)
        {
            IEnumerable<Video> similarVideos = await dataStrore.Videos.SearchBYTags(video.Tags, skip, limit);
            var tasks = similarVideos.Select(async v => await VideoProxy.Create(dataStrore, v));
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<VideoProxy>> GetPopularVideos(int skip, int limit)
        {
            var result = await dataStrore.Videos.GetPopularVideos(skip, limit);
            var tasks = result.Select(async v => await VideoProxy.Create(dataStrore, v));
            return await Task.WhenAll(tasks);
        }

        public async void AddComment(CommentDTO comment)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<CommentDTO, Comment>()
                        .ForMember(s => s.ComentatorIdString, s => s.MapFrom(scr => scr.CommentatorId))
                        .ForMember(s => s.DestinationVideoIdString, s => s.MapFrom(scr => scr.VideoId))
                        .ForMember(s => s.CommentDateTime, s => s.MapFrom(scr => scr.CommentDateTime))
                        .ForMember(s => s.Text, s => s.MapFrom(scr => scr.CommentText)));
            await dataStrore.Comments.CreateAsync(Mapper.Map<CommentDTO, Comment>(comment));
        }

        public async void DeleteVideo(VideoProxy video)
        {
            fileStore.DeletePoster(video.PosterUri);
            fileStore.DeleteVideo(video.VideoUri);
            await dataStrore.Comments.DeleteCommentsFromVideo(video.video);
        }

        public async void EstimateVideo(Video video, ViewedVideoTransferDTO transfer)
        {
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
            Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransferDTO, ViewedVideoTransfer>()
                        .ForMember(s => s.ViewerIdString, s => s.MapFrom(scr => scr.Viewer))
                        .ForMember(s => s.ViewedVideoIdString, s => s.MapFrom(scr => scr.ViewedVideo))
                        .ForMember(s => s.Status, s => s.MapFrom(scr => (DAL.Entities.ViewStatus)scr.Status))
                        .ForMember(s => s.ShowDateTime, s => s.MapFrom(scr => scr.ShowDateTime)));
            await dataStrore.ViewedVideoTransfers.CreateAsync(Mapper.Map<ViewedVideoTransferDTO, ViewedVideoTransfer>(transfer));
            await dataStrore.Videos.UpdateAsync(video);
        }
        #endregion

        #region SubscribtionLogic
        public async Task Subscribe(SubscriptionDTO subscription)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<SubscriptionDTO, Subscription>()
                        .ForMember(s => s.PublisherIdString, s => s.MapFrom(scr => scr.Publisher))
                        .ForMember(s => s.SubscriberIdString, s => s.MapFrom(scr => scr.Subscriber))
                        .ForMember(s => s.StartDate, s => s.MapFrom(scr => scr.StartDate)));
            if (!await dataStrore.Subscriptions.IsSubscriber(subscription.Publisher, subscription.Subscriber)
                && subscription.Publisher != subscription.Subscriber)
            {
                await dataStrore.Subscriptions.CreateAsync(Mapper.Map<SubscriptionDTO, Subscription>(subscription));
            }
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            await dataStrore.Subscriptions.DeleteAsync(subscriptionId);
        }
        #endregion

        #region ReportLogic
        public Task ReportVideo(string videoId)
        {
            throw new NotImplementedException();
        }

        public Task ReportComment(string CommentId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

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
using AutoMapper;

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

            Mapper.Initialize(cfg => cfg.CreateMap<Channel, DAL.Entities.Channel>()
            .ForMember("IDString", opt => opt.MapFrom(c => c.Id)));
        }

        #region ChannelLogic
        public async Task<string> CreateChannel(string userName, byte[] avatar = null, string fileExtension = null)
        {
            string avatarUri = null;
            if (avatar != null)
            {
                string name = Guid.NewGuid().ToString();
                avatarUri = fileStore.SaveAvatar(avatar, name, fileExtension);
            }
            else
            {
                avatarUri = fileStore.DefaultAvatarUri();
            }
            DAL.Entities.Channel channel = new DAL.Entities.Channel
            {
                Username = userName,
                AvatarUri = avatarUri,
            };
            return await dataStrore.Channels.CreateAsync(channel);
        }

        public Channel GetChannel(string id)
        {
            DAL.Entities.Channel channel = dataStrore.Channels.Get(id);
            return new Channel(dataStrore)
            {
                Id = channel.IDString,
                Username = channel.Username,
                AvatarUri = channel.AvatarUri,
            };
        }

        public async Task EditChannel(Channel channel, string userName, byte[] avatar = null, string fileExtension = null)
        {
            if (avatar != null)
            {
                string name = Guid.NewGuid().ToString();
                channel.AvatarUri = fileStore.SaveAvatar(avatar, name, fileExtension);
            }
            DAL.Entities.Channel newChannel = Mapper.Map<Channel, DAL.Entities.Channel>(channel);
            await dataStrore.Channels.UpdateAsync(newChannel);         
        }
        #endregion

        #region VideoLogic
        public Task<string> CreateVideo(string name, string description, string category, List<string> tags, byte[] video)
        {
            throw new NotImplementedException();
        }

        public Task<Video> GetVideo(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Video>> GetSimilarVideos(Video video)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Video>> GetPopularVideos()
        {
            throw new NotImplementedException();
        }

        public Task AddComment(string videoId, string commentatorId, string text)
        {
            throw new NotImplementedException();
        }

        public Task EstimateVideo(string video, string channel, ViewStatus status, bool isRollback = false)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SubscribtionLogic
        public Task Subscribe(string publisher, string subscriber)
        {
            throw new NotImplementedException();
        }

        public Task Unsubscribe(string subscriptionId)
        {
            throw new NotImplementedException();
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

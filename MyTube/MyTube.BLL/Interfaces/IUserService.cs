using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
using MyTube.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Interfaces
{
    public enum ViewStatus
    {
        IGNORE,
        LIKE,
        DISLIKE,
    }

    public interface IUserService
    {
        Task<string> CreateChannelAsync(string userName);
        Task<ChannelProxy> GetChannelAsync(string id);
        Task EditChannelUsernameAsync(string channelId, string username);
        Task EditChannelAvatarAsync(string channelId, string avatarPath);


        Task<string> CreateVideoAsync(
            string uploderId,
            string name, 
            string description,
            string category,
            List<string> tags, 
            string videoPath, 
            string posterPath
            );

        Task<VideoProxy> GetVideoAsync(string id);
        Task<IEnumerable<VideoProxy>> GetSimilarVideosAsync(VideoProxy video, int skip, int limit);
        Task<IEnumerable<VideoProxy>> GetPopularVideosAsync(int skip, int limit);
        void AddCommentAsync(CommentDTO comment);
        void EstimateVideoAsync(Video video, ViewedVideoTransferDTO transfer);
        Task<ViewedVideoTransferDTO> GetVideoEstimationAsync(string channel, string video);
        
        Task SubscribeAsync(SubscriptionDTO subscription);
        Task UnsubscribeAsync(string subscriptionId);

        Task ReportAsync(string link, string message);
    }
}

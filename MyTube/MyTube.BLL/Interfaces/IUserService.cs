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
            string[] tags, 
            string videoPath, 
            string posterPath
            );

        Task<VideoProxy> GetVideoAsync(string id, string UserHostAddress = null);
        Task<IEnumerable<VideoProxy>> GetVideosFromChannelAsync(string channel, int skip, int limit);
        Task<long> GetVideosFromChannelCountAsync(string channel);
        Task<IEnumerable<VideoProxy>> GetSimilarVideosAsync(VideoProxy video, int skip, int limit);
        Task<IEnumerable<VideoProxy>> GetPopularVideosAsync(int skip, int limit);
        Task<IEnumerable<CommentDTO>> GetCommentsAsync(string videoId, int skip, int limit);
        Task<long> GetCommentsCountAsync(string videoId);
        Task AddCommentAsync(CommentDTO comment);
        Task EstimateVideoAsync(ViewedVideoTransferDTO transfer);
        Task<ViewedVideoTransferDTO> GetVideoEstimationAsync(string channel, string video);
        
        Task SubscribeAsync(SubscriptionDTO subscription);
        Task UnsubscribeAsync(string publisher, string subscriber);
        Task<long> SubscribersCountAsync(string channel);
        Task<bool> IsSubscriberAsync(string publisher, string subscriber);



        Task ReportAsync(string link, string message);
    }
}

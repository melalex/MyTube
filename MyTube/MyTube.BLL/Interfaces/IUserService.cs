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
        Task DeleteChannelAsync(string id);
        Task EditChannelUsernameAsync(ChannelProxy channel, string username);
        Task EditChannelAvatarAsync(ChannelProxy channel, string avatarPath);


        Task<string> CreateVideoAsync(
            string uploderId,
            string name, 
            string description,
            string category,
            string[] tags, 
            string videoPath, 
            string posterPath
            );

        Task<IEnumerable<VideoProxy>> FulltextSearchAsync(string queryString, int skip, int limit);
        Task<long> FulltextCountSearchAsync(string queryString);
        Task<IEnumerable<VideoProxy>> CategorySearchAsync(string category, int skip, int limit);
        Task<long> CategorySearchCountAsync(string category);
        Task<IEnumerable<VideoProxy>> TagsSearchAsync(string tag, int skip, int limit);
        Task<long> TagsSearchCountAsync(string tag);

        Task<long> VideosCountAsync();

        Task<VideoProxy> GetVideoAsync(string id);
        Task<IEnumerable<VideoProxy>> GetVideosFromChannelAsync(string channel, int skip, int limit);
        Task ForEachVideoFromChannelAsync(string channel, Func<VideoProxy, Task> job);
        Task<long> GetVideosFromChannelCountAsync(string channel);
        Task<IEnumerable<VideoProxy>> GetSimilarVideosAsync(VideoProxy video, int skip, int limit);
        Task<IEnumerable<VideoProxy>> GetPopularVideosAsync(int skip, int limit);

        Task<IEnumerable<CommentDTO>> GetCommentsAsync(string videoId, int skip, int limit);
        Task<long> GetCommentsCountAsync(string videoId);
        Task AddCommentAsync(CommentDTO comment);

        Task<bool> AddView(VideoProxy video, string userHostAddress);
        Task<bool> EstimateVideoAsync(ViewedVideoTransferDTO transfer, VideoProxy video);
        Task<ViewedVideoTransferDTO> GetVideoEstimationAsync(string channel, string video);
        
        Task SubscribeAsync(SubscriptionDTO subscription);
        Task UnsubscribeAsync(string publisher, string subscriber);
        Task<long> SubscribersCountAsync(string channel);
        Task<bool> IsSubscriberAsync(string publisher, string subscriber);
        Task<IEnumerable<ChannelProxy>> SubscriptionsAsync(string channel, int skip, int limit);
        Task<long> SubscriptionsCountAsync(string channel);



        Task ReportAsync(string link, string message);
    }
}

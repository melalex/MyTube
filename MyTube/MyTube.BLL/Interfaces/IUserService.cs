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
        Task<string> CreateChannel(string userName, byte[] avatar = null);
        Task<ChannelProxy> GetChannel(string id);
        Task EditChannel(ChannelProxy channel, string userName, byte[] avatar = null);

        Task<string> CreateVideo(
            string uploderId, string name, string description, string category, List<string> tags, byte[] video, byte[] poster
            );
        Task<VideoProxy> GetVideo(string id);
        Task<IEnumerable<VideoProxy>> GetSimilarVideos(VideoProxy video, int skip, int limit);
        Task<IEnumerable<VideoProxy>> GetPopularVideos(int skip, int limit);
        void AddComment(CommentDTO comment);
        void EstimateVideo(Video video, ViewedVideoTransferDTO transfer);

        Task Subscribe(SubscriptionDTO subscription);
        Task Unsubscribe(string subscriptionId);

        Task ReportVideo(string videoId);
        Task ReportComment(string CommentId);
    }
}

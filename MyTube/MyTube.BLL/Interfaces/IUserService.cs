using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
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
        Task<Channel> GetChannel(string id);
        Task EditChannel(string channelId, string userName, byte[] avatar = null);

        Task<string> CreateVideo(string name, string description, string category, List<string> tags, byte[] video);
        Task<Video> GetVideo(string id);
        Task<IEnumerable<Video>> GetSimilarVideos(Video video);
        Task<IEnumerable<Video>> GetPopularVideos();
        Task AddComment(string videoId, string commentatorId, string text);
        Task EstimateVideo(string video, string channel, ViewStatus status, bool isRollback = false);

        Task Subscribe(string publisher, string subscriber);
        Task Unsubscribe(string subscriptionId);

        Task ReportVideo(string videoId);
        Task ReportComment(string CommentId);
    }
}

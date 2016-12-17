using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Extensions
{
    public static class VideoRepositoryExtension
    {
        public static async Task<IEnumerable<Video>> SearchByStringAsync(
            this IRepositotory<Video> video, string searchString, int skip, int limit
            )
        {
            var filter = Builders<Video>.Filter.Text(searchString);
            var options = new FindOptions<Video>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await video.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<IEnumerable<Video>> SearchBYTagsAsync(
            this IRepositotory<Video> videos, List<string> tags, int skip, int limit
            )
        {
            return await videos.Collection
                .Find(v => v.Tags.Any(tag => tags.Contains(tag)))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        public static async Task<IEnumerable<Video>> GetPopularVideosAsync(
            this IRepositotory<Video> videos, int skip, int limit
            )
        {
            Func<int, int, double> wilsonScore = (likes, dislikes) =>
            {
                int n = likes + dislikes;
                if (n == 0)
                    return 0;
                double z = 1.96;
                double phat = 1.0 * likes / n;
                return (phat + z * z / (2 * n) - z * Math.Sqrt((phat * (1 - phat) + z * z / (4 * n)) / n)) / (1 + z * z / n);
            };
            return await videos
                .Collection
                .AsQueryable()
                .OrderBy(v => wilsonScore(v.Likes, v.Dislikes))
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
        }

        public static async Task<IEnumerable<Video>> GetVideosFromChannelAsync(
            this IRepositotory<Video> videos, string channel, int skip, int limit
            )
        {
            var filter = Builders<Video>.Filter.Eq(
                c => c.Uploader, new MongoDBRef(Channel.collectionName, new ObjectId(channel))
                );
            return await videos.Collection.Find(filter).Skip(skip).Limit(limit).ToListAsync();
        }

        public static async Task<long> GetVideosFromChannelCountAsync(
            this IRepositotory<Video> videos, string channel
            )
        {
            var filter = Builders<Video>.Filter.Eq(
                c => c.Uploader, new MongoDBRef(Channel.collectionName, new ObjectId(channel))
                );
            return await videos.Collection.Find(filter).CountAsync();
        }

        #region EstimationLogic
        public static async Task AddView(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Views, 1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task AddLike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Likes, 1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task RemoveLike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Likes, -1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task RemoveLikeAndAddDislike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Likes, -1).Inc(v => v.Dislikes, 1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task AddDislike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Dislikes, 1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task RemoveDislike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Dislikes, -1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }

        public static async Task RemoveDislikeAndAddLike(this IRepositotory<Video> videos, string video)
        {
            var filter = Builders<Video>.Filter.Eq(v => v.Id, new ObjectId(video));
            var update = Builders<Video>.Update.Inc(v => v.Dislikes, -1).Inc(v => v.Likes, 1);
            await videos.Collection.FindOneAndUpdateAsync(filter, update);
        }
        #endregion
    }
}

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
    public static class HistoryRepositoryExtension
    {
        public static async Task<IEnumerable<ViewedVideoTransfer>> GetHistoryFromChannelAsync(
            this IRepositotory<ViewedVideoTransfer> history, Channel channel, int skip, int limit
            )
        {
            var filter = Builders<ViewedVideoTransfer>.Filter.Eq(v => v.Viewer, channel.DBRef);
            var options = new FindOptions<ViewedVideoTransfer>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await history.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<ViewedVideoTransfer> GetByChannelVideoAsync(
            this IRepositotory<ViewedVideoTransfer> history, string channel, string video
            )
        {
            var filter1 = Builders<ViewedVideoTransfer>.Filter.Eq(
                s => s.Viewer,
                new MongoDBRef(Channel.collectionName, new ObjectId(channel))
                );
            var filter2 = Builders<ViewedVideoTransfer>.Filter.Eq(
                s => s.ViewedVideo,
                new MongoDBRef(Video.collectionName, new ObjectId(video))
                );
            return await history.Collection.Find(filter1 & filter2).SingleOrDefaultAsync();
        }

        public static async Task DeleteHistory(
            this IRepositotory<ViewedVideoTransfer> history, string video
            )
        {
            var filter = Builders<ViewedVideoTransfer>.Filter.Eq(
                s => s.ViewedVideo,
                new MongoDBRef(Video.collectionName, new ObjectId(video))
                );
            await history.Collection.DeleteManyAsync(filter);
        }

        public static async Task<bool> IsWatched(
            this IRepositotory<ViewingHistory> history, string ip, string video
            )
        {
            var filter1 = Builders<ViewingHistory>.Filter.Eq(s => s.UserHostAddress, ip);
            var filter2 = Builders<ViewingHistory>.Filter.Eq(
                s => s.DestinationVideo,
                new MongoDBRef(Video.collectionName, new ObjectId(video))
                );
            return await history.Collection.Find(filter1 & filter2).AnyAsync();
        }

        public static async Task DeleteHistory(
            this IRepositotory<ViewingHistory> history, string video
            )
        {
            var filter = Builders<ViewingHistory>.Filter.Eq(
                s => s.DestinationVideo,
                new MongoDBRef(Video.collectionName, new ObjectId(video))
                );
            await history.Collection.DeleteManyAsync(filter);
        }
    }
}

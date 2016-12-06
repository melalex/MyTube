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
            var filter = Builders<ViewedVideoTransfer>.Filter.And(
                Builders<ViewedVideoTransfer>.Filter.Eq(
                    s => s.Viewer, new MongoDBRef(Channel.collectionName, channel)
                    ),
                Builders<ViewedVideoTransfer>.Filter.Eq(
                    s => s.ViewedVideo, new MongoDBRef(Video.collectionName, video)
                    )
                );
            return await history.Collection.Find(filter).SingleOrDefaultAsync();
        }
    }
}

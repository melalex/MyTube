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
        public static async Task<IEnumerable<ViewedVideoTransfer>> GetNotificationFromChannel(
            this IRepositotory<ViewedVideoTransfer> comments, Channel channel, int skip, int limit
            )
        {
            var filter = Builders<ViewedVideoTransfer>.Filter.Eq(v => v.Viewer, channel.DBRef);
            var options = new FindOptions<ViewedVideoTransfer>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await comments.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }
    }
}

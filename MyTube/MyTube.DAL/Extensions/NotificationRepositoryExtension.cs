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
    public static class NotificationRepositoryExtension
    {
        public static async Task<IEnumerable<Notification>> GetNotificationFromChannel(
            this IRepositotory<Notification> notification, Channel channel, int skip, int limit
            )
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.DestinationChannel, channel.DBRef);
            var options = new FindOptions<Notification>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await notification.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }
    }
}

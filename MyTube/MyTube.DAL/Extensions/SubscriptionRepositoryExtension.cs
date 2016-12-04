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
    public static class SubscriptionRepositoryExtension
    {
        public static async Task<IEnumerable<Subscription>> GetSubscribers(
            this IRepositotory<Subscription> subscription, Channel channel, int skip, int limit
            )
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.Publisher, channel.DBRef);
            var options = new FindOptions<Subscription>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await subscription.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<IEnumerable<Subscription>> GetSubscribtions(
            this IRepositotory<Subscription> subscription, Channel channel, int skip, int limit
            )
        {
            var filter = Builders<Subscription>.Filter.Eq(s => s.Subscriber, channel.DBRef);
            var options = new FindOptions<Subscription>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await subscription.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<bool> IsSubscriber(
            this IRepositotory<Subscription> subscription, string publisher, string subscriber
            )
        {
            var filter = Builders<Subscription>.Filter.And(
                Builders<Subscription>.Filter.Eq(s => s.PublisherIdString, publisher),
                Builders<Subscription>.Filter.Eq(s => s.SubscriberIdString, subscriber)
                );

            return await subscription.Collection.Find(filter).AnyAsync();
        }
    }
}

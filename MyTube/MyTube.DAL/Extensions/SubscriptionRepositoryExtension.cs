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
        public static async Task<IEnumerable<Subscription>> GetSubscribersAsync(
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

        public static async Task<IEnumerable<Subscription>> GetSubscribtionsAsync(
            this IRepositotory<Subscription> subscription, string channel, int skip, int limit
            )
        {
            var filter = Builders<Subscription>.Filter.Eq(
                s => s.Subscriber,
                new MongoDBRef(Channel.collectionName, new ObjectId(channel))
                );
            var options = new FindOptions<Subscription>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await subscription.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<long> GetSubscribtionsCountAsync(
            this IRepositotory<Subscription> subscription, string channel
            )
        {
            var filter = Builders<Subscription>.Filter.Eq(
                s => s.Subscriber,
                new MongoDBRef(Channel.collectionName, new ObjectId(channel))
                );
            return await subscription.Collection.Find(filter).CountAsync();
        }

        public static async Task UnSubscribeAsync(
            this IRepositotory<Subscription> subscription, string publisher, string subscriber
            )
        {
            var filter1 = Builders<Subscription>.Filter.Eq(
                s => s.Publisher,
                new MongoDBRef(Channel.collectionName, new ObjectId(publisher))
                );
            var filter2 = Builders<Subscription>.Filter.Eq(
                s => s.Subscriber,
                new MongoDBRef(Channel.collectionName, new ObjectId(subscriber))
                );

            await subscription.Collection.DeleteManyAsync(filter1 & filter2);
        }

        public static async Task<bool> IsSubscriberAsync(
            this IRepositotory<Subscription> subscription, string publisher, string subscriber
            )
        {
            var filter1 = Builders<Subscription>.Filter.Eq(
                s => s.Publisher,
                new MongoDBRef(Channel.collectionName, new ObjectId(publisher))
                );
            var filter2 = Builders<Subscription>.Filter.Eq(
                s => s.Subscriber,
                new MongoDBRef(Channel.collectionName, new ObjectId(subscriber))
                );

            return await subscription.Collection.Find(filter1 & filter2).AnyAsync();
        }

        public static async Task<long> GetSubscribersCountAsync(
            this IRepositotory<Subscription> subscription, string publisher
            )
        {
            var filter = Builders<Subscription>.Filter.Eq(
                s => s.Publisher,
                new MongoDBRef(Channel.collectionName, new ObjectId(publisher))
                );

            return await subscription.Collection.Find(filter).CountAsync();
        }
    }
}

using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MyTube.DAL.Repositories
{
    class ChannelRepository : IRepository<Channel>
    {
        private IMongoCollection<Channel> collection;
        private const string collectionName = "Channels";

        public ChannelRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<Channel>(collectionName); ;
        }

        public async void Create(Channel item)
        {
            await collection.InsertOneAsync(item);
        }

        public async void Delete(ObjectId id)
        {
            var filter = Builders<Channel>.Filter.Eq(o => o.Id, id);
            await collection.DeleteOneAsync(filter);
        }

        public Task<IEnumerable<Channel>> Find(Func<Channel, bool> predicate)
        {
            return Task.Run(() => collection.AsQueryable().Where(predicate));
        }

        public async Task<Channel> Get(ObjectId id)
        {
            var filter = Builders<Channel>.Filter.Eq(o => o.Id, id);
            var result = await collection.Find(filter).ToListAsync();
            return result.First();
        }

        public IEnumerable<Channel> GetAll()
        {
            return collection.AsQueryable();
        }

        public async void Update(Channel item)
        {
            var filter = Builders<Channel>.Filter.Eq(o => o.Id, item.Id);
            await collection.ReplaceOneAsync(filter, item);
        }
    }
}

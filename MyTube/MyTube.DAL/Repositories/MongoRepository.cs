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
    class MongoRepository<TDocument> : IRepositotory<TDocument> where TDocument : IEntitie
    {
        private IMongoCollection<TDocument> collection;
        private const string collectionName = "Channels";

        public MongoRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<TDocument>(collectionName); ;
        }

        public async void Create(TDocument item)
        {
            await collection.InsertOneAsync(item);
        }

        public async void Delete(ObjectId id)
        {
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, id);
            await collection.DeleteOneAsync(filter);
        }

        public Task<IEnumerable<TDocument>> Find(Func<TDocument, bool> predicate)
        {
            return Task.Run(() => collection.AsQueryable().Where(predicate));
        }

        public async Task<TDocument> Get(ObjectId id)
        {
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, id);
            var result = await collection.Find(filter).ToListAsync();
            return result.First();
        }

        public IEnumerable<TDocument> GetAll()
        {
            return collection.AsQueryable();
        }

        public async void Update(TDocument item)
        {
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, item.Id);
            await collection.ReplaceOneAsync(filter, item);
        }
    }
}

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
    public class MongoRepository<TDocument> : IRepositotory<TDocument> where TDocument : Entitie
    {
        private IMongoCollection<TDocument> collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            collection = database.GetCollection<TDocument>(collectionName); ;
        }

        public async Task Create(TDocument item)
        {
            await collection.InsertOneAsync(item);
        }

        public async Task Delete(string id)
        {
            ObjectId documentId = new ObjectId(id);
            await collection.DeleteOneAsync(a => a.Id == documentId);
        }

        public Task<IEnumerable<TDocument>> Find(Func<TDocument, bool> predicate)
        {
            return Task.Run(() => collection.AsQueryable().Where(predicate));
        }

        public async Task<TDocument> Get(string id)
        {
            ObjectId documentId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, documentId);
            var result = await collection.Find(filter).ToListAsync();
            return result.First();
        }

        public IEnumerable<TDocument> GetAll()
        {
            return collection.AsQueryable();
        }

        public async Task Update(TDocument item)
        {
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, item.Id);
            await collection.ReplaceOneAsync(filter, item);
        }
    }
}

using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MyTube.DAL.Repositories
{
    public class MongoRepository<TDocument> : IRepositotory<TDocument> where TDocument : Entitie
    {
        public IMongoCollection<TDocument> Collection { get; private set; }

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            Collection = database.GetCollection<TDocument>(collectionName); ;
        }

        public async Task<string> CreateAsync(TDocument item)
        {
            await Collection.InsertOneAsync(item);
            return item.IdString;
        }

        public async Task DeleteAsync(string id)
        {
            ObjectId documentId = new ObjectId(id);
            await Collection.DeleteOneAsync(a => a.Id == documentId);
        }

        public IEnumerable<TDocument> Find(Func<TDocument, bool> predicate)
        {
            return Collection.AsQueryable().Where(predicate);
        }

        public async Task<TDocument> Get(string id)
        {
            ObjectId documentId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, documentId);
            return await Collection.Find(filter).FirstAsync();
        }

        public IEnumerable<TDocument> GetAll()
        {
            return Collection.AsQueryable();
        }

        public async Task UpdateAsync(TDocument item)
        {
            var filter = Builders<TDocument>.Filter.Eq(o => o.Id, item.Id);
            await Collection.ReplaceOneAsync(filter, item);
        }
    }
}

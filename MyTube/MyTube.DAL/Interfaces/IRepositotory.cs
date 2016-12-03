using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.Entities;
using MongoDB.Driver;

namespace MyTube.DAL.Interfaces
{
    public interface IRepositotory<TDocument> where TDocument : Entitie
    {
        IMongoCollection<TDocument> Collection { get; }

        Task<string> CreateAsync(TDocument item);
        Task DeleteAsync(string id);
        IEnumerable<TDocument> Find(Func<TDocument, bool> predicate);
        TDocument Get(string id);
        IEnumerable<TDocument> GetAll();
        Task UpdateAsync(TDocument item);
    }
}

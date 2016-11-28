using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Interfaces
{
    public interface IRepositotory<TDocument> where TDocument : IEntitie
    {
        Task Create(TDocument item);
        Task Delete(ObjectId id);
        Task<IEnumerable<TDocument>> Find(Func<TDocument, bool> predicate);
        Task<TDocument> Get(ObjectId id);
        IEnumerable<TDocument> GetAll();
        void Update(TDocument item);
    }
}

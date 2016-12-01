using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.Entities;

namespace MyTube.DAL.Interfaces
{
    public interface IRepositotory<TDocument> where TDocument : Entitie
    {
        Task Create(TDocument item);
        Task Delete(string id);
        Task<IEnumerable<TDocument>> Find(Func<TDocument, bool> predicate);
        Task<TDocument> Get(string id);
        IEnumerable<TDocument> GetAll();
        Task Update(TDocument item);
    }
}

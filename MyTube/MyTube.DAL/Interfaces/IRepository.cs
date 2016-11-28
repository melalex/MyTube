using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Interfaces
{
    interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        Task<T> Get(ObjectId id);
        Task<IEnumerable<T>> Find(Func<T, bool> predicate);
        void Create(T item);
        void Update(T item);
        void Delete(ObjectId id);
    }
}

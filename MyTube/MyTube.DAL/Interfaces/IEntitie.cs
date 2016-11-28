using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Interfaces
{
    public interface IEntitie
    {
        ObjectId Id { get; set; }
    }
}

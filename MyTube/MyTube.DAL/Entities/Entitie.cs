using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Entities
{
    public class Entitie
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }
    }
}

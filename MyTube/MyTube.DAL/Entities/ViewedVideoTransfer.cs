using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Entities
{
    public enum ViewStatus
    {
        IGNORE,
        LIKE,
        DISLIKE,
    }

    public class ViewedVideoTransfer : Entitie
    {
        public MongoDBRef Channel { get; set; }

        public MongoDBRef Video { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public ViewStatus Status { get; set; }

        public DateTimeOffset ShowDateTime { get; set; }
    }
}

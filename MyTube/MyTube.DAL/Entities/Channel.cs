using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Entities
{
    public class Channel : IEntitie
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Uri AvatarUri { get; set; }

        public List<MongoDBRef> ViewedVideos { get; set; }

        public List<MongoDBRef> Notifications { get; set; }

        public List<MongoDBRef> Subscribers { get; set; }
    }
}

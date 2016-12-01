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
    public class Video : Entitie
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        
        public string VideoUrl { get; set; }

        public MongoDBRef Uploder { get; set; }

        public DateTimeOffset UploadDate { get; set; }

        public List<string> Tags { get; set; }

        public string Category { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Views { get; set; }
    }
}

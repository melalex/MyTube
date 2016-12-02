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
    public class Comment : Entitie
    {
        public MongoDBRef DestinationVideo { get; set; }

        public MongoDBRef Comentator { get; set; }

        public DateTimeOffset CommentDateTime { get; set; }

        public string Text { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "Comments";

        [BsonIgnore]
        public string DestinationVideoIdString
        {
            get
            {
                return DestinationVideo.Id.ToString();
            }
            set
            {
                DestinationVideo = new MongoDBRef(Video.collectionName, value);
            }
        }

        [BsonIgnore]
        public string ComentatorIdString
        {
            get
            {
                return Comentator.Id.ToString();
            }
            set
            {
                Comentator = new MongoDBRef(Channel.collectionName, value);
            }
        }

        [BsonIgnore]
        public MongoDBRef DBRef
        {
            get
            {
                return new MongoDBRef(collectionName, Id);
            }
        }
    }
}

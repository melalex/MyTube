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
    public class ViewingHistory : Entitie
    {
        public MongoDBRef DestinationVideo { get; set; }
        public string UserHostAddress { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "ViewingHistory";

        [BsonIgnore]
        public string DestinationVideoIdString
        {
            get
            {
                return DestinationVideo.Id.ToString();
            }
            set
            {
                DestinationVideo = new MongoDBRef(Video.collectionName, new ObjectId(value));
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

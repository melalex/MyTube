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
        public MongoDBRef Viewer { get; set; }

        public MongoDBRef ViewedVideo { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public ViewStatus Status { get; set; }

        public DateTimeOffset ShowDateTime { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "ViewedVideoTransfers";

        [BsonIgnore]
        public string ViewerIdString
        {
            get
            {
                return Viewer.Id.ToString();
            }
            set
            {
                Viewer = new MongoDBRef(Channel.collectionName, new ObjectId(value));
            }
        }

        [BsonIgnore]
        public string ViewedVideoIdString
        {
            get
            {
                return ViewedVideo.Id.ToString();
            }
            set
            {
                ViewedVideo = new MongoDBRef(Video.collectionName, new ObjectId(value));
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

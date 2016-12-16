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
        public string Name { get; set; }
        
        public string VideoUrl { get; set; }

        public string PosterUrl { get; set; }

        public string Description { get; set; }

        public MongoDBRef Uploder { get; set; }

        public DateTimeOffset UploadDate { get; set; }

        public List<string> Tags { get; set; }

        public string Category { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Views { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "Videos";

        [BsonIgnore]
        public string UploderIdString
        {
            get
            {
                return Uploder.Id.ToString();
            }
            set
            {
                Uploder = new MongoDBRef(Channel.collectionName, new ObjectId(value));
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

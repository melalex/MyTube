using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Entities
{
    public class Notification : Entitie
    {
        public MongoDBRef Channel { get; set; }

        public DateTimeOffset NotificationDateTime { get; set; }

        public string Text { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "Notifications";

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

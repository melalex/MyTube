using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Entities
{
    public class Subscription : Entitie
    {
        public MongoDBRef Publisher { get; set; }

        public MongoDBRef Subscriber { get; set; }

        public DateTimeOffset StartDate { get; set; }

        [BsonIgnore]
        public static string collectionName { get; private set; } = "Subscriptions";

        [BsonIgnore]
        public string PublisherIdString
        {
            get
            {
                return Publisher.Id.ToString();
            }
            set
            {
                Publisher = new MongoDBRef(Channel.collectionName, value);
            }
        }

        [BsonIgnore]
        public string SubscriberIdString
        {
            get
            {
                return Subscriber.Id.ToString();
            }
            set
            {
                Subscriber = new MongoDBRef(Channel.collectionName, value);
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

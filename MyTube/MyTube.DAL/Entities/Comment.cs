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
        public MongoDBRef Comentator { get; set; }

        public DateTimeOffset CommentDateTime { get; set; }

        public string Text { get; set; }
    }
}

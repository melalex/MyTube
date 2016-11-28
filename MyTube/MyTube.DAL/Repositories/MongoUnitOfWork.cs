using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.Entities;
using MongoDB.Driver;

namespace MyTube.DAL.Repositories
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private const string dataBaseName = "MyTube";
        private const string channelsCollectionName = "Channels";
        private const string videosCollectionName = "Videos";

        private IMongoDatabase database;

        private MongoRepository<Channel> _chanels = null;
        private MongoRepository<Video> _videos = null;

        public MongoUnitOfWork(MongoClient mongoClient)
        {
            database = mongoClient.GetDatabase(dataBaseName);
        }

        public IRepositotory<Channel> Channels
        {
            get
            {
                if (_chanels == null)
                {
                    _chanels = new MongoRepository<Channel>(database, channelsCollectionName);
                }
                return _chanels;
            }
        }

        public IRepositotory<Video> Videos
        {
            get
            {
                if (_videos == null)
                {
                    _videos = new MongoRepository<Video>(database, videosCollectionName);
                }
                return _videos;
            }
        }
    }
}

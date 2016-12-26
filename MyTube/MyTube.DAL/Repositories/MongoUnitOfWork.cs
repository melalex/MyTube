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

        private IMongoDatabase database;

        private MongoRepository<Channel> _chanels = null;
        private MongoRepository<Video> _videos = null;
        private MongoRepository<Comment> _comments = null;
        private MongoRepository<Notification> _notifications = null;
        private MongoRepository<Subscription> _subscriptions = null;
        private MongoRepository<ViewedVideoTransfer> _viewedVideoTransfers = null;
        private MongoRepository<ViewingHistory> _viewingHistories = null;

        public MongoUnitOfWork(MongoClient mongoClient)
        {
            database = mongoClient.GetDatabase(dataBaseName);
        }

        public MongoUnitOfWork(string connectionString)
        {
            MongoClient mongoClient = new MongoClient(connectionString);
            database = mongoClient.GetDatabase(dataBaseName);
        }

        public IRepositotory<Channel> Channels
        {
            get
            {
                if (_chanels == null)
                {
                    _chanels = new MongoRepository<Channel>(database, Channel.collectionName);
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
                    _videos = new MongoRepository<Video>(database, Video.collectionName);
                }
                return _videos;
            }
        }

        public IRepositotory<Comment> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new MongoRepository<Comment>(database, Comment.collectionName);
                }
                return _comments;
            }
        }

        public IRepositotory<Notification> Notifications
        {
            get
            {
                if (_notifications == null)
                {
                    _notifications = new MongoRepository<Notification>(database, Notification.collectionName);
                }
                return _notifications;
            }
        }

        public IRepositotory<Subscription> Subscriptions
        {
            get
            {
                if (_subscriptions == null)
                {
                    _subscriptions = new MongoRepository<Subscription>(database, Subscription.collectionName);
                }
                return _subscriptions;
            }
        }

        public IRepositotory<ViewedVideoTransfer> ViewedVideoTransfers
        {
            get
            {
                if (_viewedVideoTransfers == null)
                {
                    _viewedVideoTransfers = new MongoRepository<ViewedVideoTransfer>(database, ViewedVideoTransfer.collectionName);
                }
                return _viewedVideoTransfers;
            }
        }

        public IRepositotory<ViewingHistory> ViewingHistories
        {
            get
            {
                if (_viewingHistories == null)
                {
                    _viewingHistories = new MongoRepository<ViewingHistory>(database, ViewingHistory.collectionName);
                }
                return _viewingHistories;
            }
        }
    }
}

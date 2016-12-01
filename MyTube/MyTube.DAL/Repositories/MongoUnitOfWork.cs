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
        private const string commentsCollectionName = "Comments";
        private const string notificationsCollectionName = "Notifications";
        private const string subscriptionsCollectionName = "subscriptionsCollectionName";
        private const string viewedVideoTransfersCollectionName = "ViewedVideoTransfers";

        private IMongoDatabase database;

        private MongoRepository<Channel> _chanels = null;
        private MongoRepository<Video> _videos = null;
        private MongoRepository<Comment> _comments = null;
        private MongoRepository<Notification> _notifications = null;
        private MongoRepository<Subscription> _subscriptions = null;
        private MongoRepository<ViewedVideoTransfer> _viewedVideoTransfers = null;

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

        public IRepositotory<Comment> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new MongoRepository<Comment>(database, commentsCollectionName);
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
                    _notifications = new MongoRepository<Notification>(database, notificationsCollectionName);
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
                    _subscriptions = new MongoRepository<Subscription>(database, subscriptionsCollectionName);
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
                    _viewedVideoTransfers = new MongoRepository<ViewedVideoTransfer>(database, viewedVideoTransfersCollectionName);
                }
                return _viewedVideoTransfers;
            }
        }
    }
}

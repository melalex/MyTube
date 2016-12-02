using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.BLL.DTO;
using MyTube.DAL.Interfaces;
using MyTube.BLL.Interfaces;

namespace MyTube.BLL.BusinessEntities
{
    public class Channel
    {
        private IUnitOfWork database;

        public Channel(IUnitOfWork database)
        {
            this.database = database;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string AvatarUri { get; set; }

        public IEnumerable<SubscriptionDTO> Subscribers
        {
            get
            {
                var subscribers = database.Subscriptions.GetAll();
                return subscribers.Where((subscription) => subscription.PublisherIdString == Id).
                    Select((subscription) => new SubscriptionDTO
                    {
                        Id = subscription.IDString,
                        SubscriptionDate = subscription.StartDate,
                    });
            }
        }

        public IEnumerable<SubscriptionDTO> Subscriptions
        {
            get
            {
                var subscriptions = database.Subscriptions.GetAll();
                return subscriptions.Where((subscribtion) => subscribtion.SubscriberIdString == Id).
                    Select((subscribtion) => new SubscriptionDTO
                    {
                        Id = subscribtion.IDString,
                        SubscriptionDate = subscribtion.StartDate,
                    });
            }
            
        }

        public IEnumerable<NotificationDTO> Notifications
        {
            get
            {
                var notifications = database.Notifications.GetAll();
                return notifications.Where((notification) => notification.DestinationChannelIdString == Id).
                    Select((notification) => new NotificationDTO
                    {
                        Id = notification.IDString,
                        Text = notification.Text,
                        Link = notification.Link,
                    });
            }
        }

        public IEnumerable<ViewedVideoTransferDTO> History
        {
            get
            {
                var viewedVideos = database.ViewedVideoTransfers.GetAll();
                return viewedVideos.Where((viewedVideo) => viewedVideo.ViewerIdString == Id).
                    Select((viewedVideo) => new ViewedVideoTransferDTO
                    {
                        Id = viewedVideo.IDString,
                        ViewDateTime = viewedVideo.ShowDateTime,
                        Status = (ViewStatus)viewedVideo.Status,
                        VideoId = viewedVideo.ViewedVideoIdString,
                    });
            }
        }
    }
}

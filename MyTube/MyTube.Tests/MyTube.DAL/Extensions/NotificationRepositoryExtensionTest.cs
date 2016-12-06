using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Configuration;
using MyTube.DAL.Repositories;
using MyTube.DAL.Entities;
using MyTube.DAL.Extensions;
using System.Threading.Tasks;
using System.Linq;

namespace MyTube.Tests.MyTube.DAL.Extensions
{
    [TestClass]
    public class NotificationRepositoryExtensionTest
    {
        private MongoClient client;

        public NotificationRepositoryExtensionTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task NotificationRepositoryExtension__GetNotificationFromChannelAsync__Got()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            Notification notification1 = new Notification
            {
                DestinationChannel = channel1.DBRef,
                NotificationDateTime = DateTimeOffset.Now,
                Text = "notification1",
                Link = "link",
            };
            Notification notification2 = new Notification
            {
                DestinationChannel = channel1.DBRef,
                NotificationDateTime = DateTimeOffset.Now,
                Text = "notification2",
                Link = "link",
            };
            Notification notification3 = new Notification
            {
                DestinationChannel = channel1.DBRef,
                NotificationDateTime = DateTimeOffset.Now,
                Text = "notification3",
                Link = "link",
            };
            await unitOfWork.Notifications.CreateAsync(notification1);
            await unitOfWork.Notifications.CreateAsync(notification2);
            await unitOfWork.Notifications.CreateAsync(notification3);

            try
            {
                // Act
                var result = await unitOfWork.Notifications.GetNotificationFromChannelAsync(channel1, 0, 20);

                // Assert
                long count = result.Count();
                Assert.AreEqual(count, 3);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Notifications.Collection.DeleteOne(a => a.Id == notification1.Id);
                unitOfWork.Notifications.Collection.DeleteOne(a => a.Id == notification2.Id);
                unitOfWork.Notifications.Collection.DeleteOne(a => a.Id == notification3.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
            }
        }
    }
}

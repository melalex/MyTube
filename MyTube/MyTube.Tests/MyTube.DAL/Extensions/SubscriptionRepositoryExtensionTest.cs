using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Configuration;
using System.Threading.Tasks;
using MyTube.DAL.Repositories;
using MyTube.DAL.Entities;
using MyTube.DAL.Extensions;
using System.Linq;

namespace MyTube.Tests.MyTube.DAL.Extensions
{
    [TestClass]
    public class SubscriptionRepositoryExtensionTest
    {
        private MongoClient client;

        public SubscriptionRepositoryExtensionTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task SubscriptionRepositoryExtension__GetSubscribersAsync__Got()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel2 = new Channel
            {
                Username = "belalex",
                AvatarUri = "http://www.bierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            await unitOfWork.Channels.CreateAsync(channel2);

            Subscription subscription1 = new Subscription
            {
                Publisher = channel1.DBRef,
                Subscriber = channel2.DBRef,
                StartDate = DateTimeOffset.Now,
            };
            await unitOfWork.Subscriptions.CreateAsync(subscription1);

            try
            {
                // Act
                var result = await unitOfWork.Subscriptions.GetSubscribersAsync(channel1, 0, 20);

                // Assert
                long count = result.Count();
                Assert.AreEqual(count, 1);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Subscriptions.Collection.DeleteOne(a => a.Id == subscription1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
                await unitOfWork.Channels.DeleteAsync(channel2.Id.ToString());
            }
        }

        [TestMethod]
        public async Task SubscriptionRepositoryExtension__GetSubscribtionsAsync__Got()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel2 = new Channel
            {
                Username = "belalex",
                AvatarUri = "http://www.bierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            await unitOfWork.Channels.CreateAsync(channel2);

            Subscription subscription1 = new Subscription
            {
                Publisher = channel1.DBRef,
                Subscriber = channel2.DBRef,
                StartDate = DateTimeOffset.Now,
            };
            await unitOfWork.Subscriptions.CreateAsync(subscription1);

            try
            {
                // Act
                var result = await unitOfWork.Subscriptions.GetSubscribtionsAsync(channel2, 0, 20);

                // Assert
                long count = result.Count();
                Assert.AreEqual(count, 1);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Subscriptions.Collection.DeleteOne(a => a.Id == subscription1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
                await unitOfWork.Channels.DeleteAsync(channel2.Id.ToString());
            }
        }

        [TestMethod]
        public async Task SubscriptionRepositoryExtension__IsSubscriberAsync__True()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel2 = new Channel
            {
                Username = "belalex",
                AvatarUri = "http://www.bierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            await unitOfWork.Channels.CreateAsync(channel2);

            Subscription subscription1 = new Subscription
            {
                Publisher = channel1.DBRef,
                Subscriber = channel2.DBRef,
                StartDate = DateTimeOffset.Now,
            };
            await unitOfWork.Subscriptions.CreateAsync(subscription1);

            try
            {
                // Act
                var result = await unitOfWork.Subscriptions.IsSubscriberAsync(channel1.IdString, channel2.IdString);

                // Assert
                Assert.IsTrue(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Subscriptions.Collection.DeleteOne(a => a.Id == subscription1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
                await unitOfWork.Channels.DeleteAsync(channel2.Id.ToString());
            }
        }

        [TestMethod]
        public async Task SubscriptionRepositoryExtension__IsSubscriberAsync__False()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel2 = new Channel
            {
                Username = "belalex",
                AvatarUri = "http://www.bierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            await unitOfWork.Channels.CreateAsync(channel2);

            Subscription subscription1 = new Subscription
            {
                Publisher = channel1.DBRef,
                Subscriber = channel2.DBRef,
                StartDate = DateTimeOffset.Now,
            };
            await unitOfWork.Subscriptions.CreateAsync(subscription1);

            try
            {
                // Act
                var result = await unitOfWork.Subscriptions.IsSubscriberAsync(channel2.IdString, channel1.IdString);

                // Assert
                Assert.IsFalse(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Subscriptions.Collection.DeleteOne(a => a.Id == subscription1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
                await unitOfWork.Channels.DeleteAsync(channel2.Id.ToString());
            }
        }
    }
}

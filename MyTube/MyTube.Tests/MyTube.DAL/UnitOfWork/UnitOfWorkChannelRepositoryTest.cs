﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTube.DAL.Interfaces;
using MyTube.DAL.Repositories;
using MyTube.DAL.Entities;
using MongoDB.Driver;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MyTube.Tests.MyTube.DAL
{
    [TestClass]
    public class UnitOfWorkChannelRepository
    {
        private MongoClient client;

        public UnitOfWorkChannelRepository()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task ChannelRepository__Create__Created()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Channel> channels = client.GetDatabase("MyTube").GetCollection<Channel>("Channels");
            Channel chanel = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"              
            };

            try
            {
                // Act
                await unitOfWork.Channels.CreateAsync(chanel);

                // Assert
                var filter = Builders<Channel>.Filter.Eq(o => o.Id, chanel.Id);
                long result = channels.Find(filter).Count();
                Assert.AreEqual(result, 1);
            }
            catch
            {
                throw;
            }
            finally
            {
                channels.DeleteOne(a => a.Id == chanel.Id);
            }

        }

        [TestMethod]
        public async Task ChannelRepository__Delete__Deleted()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Channel> chanels = client.GetDatabase("MyTube").GetCollection<Channel>("Channels");
            Channel channel = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };

            // Act
            await unitOfWork.Channels.CreateAsync(channel);
            var filter = Builders<Channel>.Filter.Eq(o => o.Id, channel.Id);
            bool created = chanels.Find(filter).Count() == 1;
            await unitOfWork.Channels.DeleteAsync(channel.Id.ToString());

            // Assert
            long result = chanels.Find(filter).Count();
            Assert.IsTrue(created);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public async Task ChannelRepository__Get__Geted()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Channel> channels = client.GetDatabase("MyTube").GetCollection<Channel>("Channels");
            Channel channel = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };

            try
            {
                // Act
                await unitOfWork.Channels.CreateAsync(channel);
                var filter = Builders<Channel>.Filter.Eq(o => o.Id, channel.Id);
                Channel anotherChennel = await unitOfWork.Channels.Get(channel.Id.ToString());

                // Assert
                Assert.AreEqual(anotherChennel.Id, channel.Id);
            }
            catch
            {
                throw;
            }
            finally
            {
                channels.DeleteOne(a => a.Id == channel.Id);
            }
        }

        [TestMethod]
        public async Task ChannelRepository__Find__Found()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Channel> channels = client.GetDatabase("MyTube").GetCollection<Channel>("Channels");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel2 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel3 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            Channel channel4 = new Channel
            {
                Username = "belalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            await unitOfWork.Channels.CreateAsync(channel2);
            await unitOfWork.Channels.CreateAsync(channel3);
            await unitOfWork.Channels.CreateAsync(channel4);

            try
            {
                // Act
                
                var result = unitOfWork.Channels.Find((Channel channel) => channel.Username == "melalex");
                long count = result.Count();

                // Assert
                Assert.AreEqual(count, 3);
            }
            catch
            {
                throw;
            }
            finally
            {
                channels.DeleteOne(a => a.Id == channel1.Id);
                channels.DeleteOne(a => a.Id == channel2.Id);
                channels.DeleteOne(a => a.Id == channel3.Id);
                channels.DeleteOne(a => a.Id == channel4.Id);
            }
        }

        [TestMethod]
        public async Task ChannelRepository__Update__Updated()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Channel> channels = client.GetDatabase("MyTube").GetCollection<Channel>("Channels");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUri = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);

            try
            {
                // Act
                channel1.Username = "balex";
                channel1.AvatarUri = "http://www.example.com";
                await unitOfWork.Channels.UpdateAsync(channel1);

                // Assert
                Channel anotherChennel = await unitOfWork.Channels.Get(channel1.Id.ToString());
                Assert.AreEqual(channel1.Username, anotherChennel.Username);
                Assert.AreEqual(channel1.AvatarUri, anotherChennel.AvatarUri);
            }
            catch
            {
                throw;
            }
            finally
            {
                channels.DeleteOne(a => a.Id == channel1.Id);
            }
        }
    }
}

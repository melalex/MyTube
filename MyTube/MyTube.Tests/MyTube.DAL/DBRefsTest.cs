using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTube.DAL.Repositories;
using MyTube.DAL.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;

namespace MyTube.Tests.MyTube.DAL
{
    [TestClass]
    public class DBRefsTest
    {
        private MongoClient client;

        public DBRefsTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task Video__DBRef()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.CreateAsync(channel1);
            DateTimeOffset UploadDate = DateTimeOffset.Now;
            Video video = new Video
            {
                Name = "Name",
                VideoUrl = "VideoUrl",
                Description = "Description",
                Uploder = new MongoDBRef("Channels", channel1.Id),
                UploadDate = UploadDate,
                Tags = new List<string>
                {
                    "tag1",
                    "tag2",
                },
                Category = "Category",
                Likes = 322,
                Dislikes = 228,
                Views = 100,
            };
            await unitOfWork.Videos.CreateAsync(video);

            // Act
            MongoDBRef videoRef = video.DBRef;
            MongoDBRef channelRef = channel1.DBRef;

            // Assert
            Assert.AreEqual(videoRef.CollectionName, Video.collectionName);
            Assert.AreEqual(videoRef.Id, video.Id);
            Assert.AreEqual(channelRef.CollectionName, Channel.collectionName);
            Assert.AreEqual(channelRef.Id, channel1.Id);

            await unitOfWork.Videos.DeleteAsync(video.Id.ToString());
            await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System.Configuration;
using MyTube.DAL.Entities;
using MyTube.DAL.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MyTube.Tests.MyTube.DAL.UnitOfWork
{
    [TestClass]
    public class UnitOfWorkVideoRepository
    {
        private MongoClient client;

        public UnitOfWorkVideoRepository()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task VideoRepository__Create__Created()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.Create(channel1);
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

            // Act
            await unitOfWork.Videos.Create(video);

            // Assert
            var filter = Builders<Video>.Filter.Eq(o => o.Id, video.Id);
            Video anotherVideo = videos.FindOneAndDelete(filter);
            Assert.AreEqual(video.Name, anotherVideo.Name);
            Assert.AreEqual(video.VideoUrl, anotherVideo.VideoUrl);
            Assert.AreEqual(video.Description, anotherVideo.Description);
            Assert.AreEqual(video.Uploder, anotherVideo.Uploder);
            Assert.AreEqual(video.UploadDate, anotherVideo.UploadDate);
            Assert.AreEqual(video.Category, anotherVideo.Category);
            Assert.AreEqual(video.Likes, anotherVideo.Likes);
            Assert.AreEqual(video.Dislikes, anotherVideo.Dislikes);
            Assert.AreEqual(video.Views, anotherVideo.Views);

            await unitOfWork.Channels.Delete(channel1.Id.ToString());
        }

        [TestMethod]
        public async Task VideoRepository__Delete__Deleted()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.Create(channel1);
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
            await unitOfWork.Videos.Create(video);

            // Act
            await unitOfWork.Videos.Delete(video.Id.ToString());

            // Assert
            var filter = Builders<Video>.Filter.Eq(o => o.Id, video.Id);
            long result = videos.Find(filter).Count();
            Assert.AreEqual(result, 0);

            await unitOfWork.Channels.Delete(channel1.Id.ToString());
        }

        [TestMethod]
        public async Task VideoRepository__Get__Geted()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.Create(channel1);
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
            await unitOfWork.Videos.Create(video);

            // Act
            Video anotherVideo = await unitOfWork.Videos.Get(video.Id.ToString());

            // Assert
            Assert.AreEqual(anotherVideo.Id, video.Id);
            videos.DeleteOne(a => a.Id == video.Id);

            await unitOfWork.Channels.Delete(channel1.Id.ToString());
        }

        [TestMethod]
        public async Task VideoRepository__Find__Found()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.Create(channel1);
            DateTimeOffset UploadDate = DateTimeOffset.Now;
            Video video1 = new Video
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
                Category = "Category1",
                Likes = 322,
                Dislikes = 228,
                Views = 100,
            };
            Video video2 = new Video
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
                Category = "Category1",
                Likes = 322,
                Dislikes = 228,
                Views = 100,
            };
            Video video3 = new Video
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
                Category = "Category2",
                Likes = 322,
                Dislikes = 228,
                Views = 100,
            };
            await unitOfWork.Videos.Create(video1);
            await unitOfWork.Videos.Create(video2);
            await unitOfWork.Videos.Create(video3);

            // Act
            var result = await unitOfWork.Videos.Find((Video video) => video.Category == "Category1");

            // Assert
            long count = result.Count();
            Assert.AreEqual(count, 2);
            videos.DeleteOne(a => a.Id == video1.Id);
            videos.DeleteOne(a => a.Id == video2.Id);
            videos.DeleteOne(a => a.Id == video3.Id);
            await unitOfWork.Channels.Delete(channel1.Id.ToString());
        }

        [TestMethod]
        public async Task VideoRepository__Update__Updated()
        {
            // Arrange
            MongoUnitOfWork unitOfWork = new MongoUnitOfWork(client);
            IMongoCollection<Video> videos = client.GetDatabase("MyTube").GetCollection<Video>("Videos");
            Channel channel1 = new Channel
            {
                Username = "melalex",
                AvatarUrl = "http://www.pierobon.org/iis/review1.htm"
            };
            await unitOfWork.Channels.Create(channel1);
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
            await unitOfWork.Videos.Create(video);

            // Act
            video.Category = "Another";
            await unitOfWork.Videos.Update(video);

            // Assert
            Video anotherVideo = await unitOfWork.Videos.Get(video.Id.ToString());
            Assert.AreEqual(video.Category, anotherVideo.Category);

            videos.DeleteOne(a => a.Id == video.Id);
            await unitOfWork.Channels.Delete(channel1.Id.ToString());
        }
    }
}

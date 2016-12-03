using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTube.DAL.Repositories;
using MongoDB.Driver;
using MyTube.DAL.Entities;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using MyTube.DAL.Extensions;

namespace MyTube.Tests.MyTube.DAL.Extensions
{
    [TestClass]
    public class VideoRepositoryExtension
    {
        private MongoClient client;

        public VideoRepositoryExtension()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task VideoRepository_Search_Found()
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
            DateTimeOffset UploadDate = DateTimeOffset.Now;
            Video video1 = new Video
            {
                Name = "searchStr",
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
                Description = "Description searchStr asd",
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
                    "searchStr",
                },
                Category = "Category2",
                Likes = 322,
                Dislikes = 228,
                Views = 100,
            };
            Video video4 = new Video
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
            await unitOfWork.Videos.CreateAsync(video1);
            await unitOfWork.Videos.CreateAsync(video2);
            await unitOfWork.Videos.CreateAsync(video3);
            await unitOfWork.Videos.CreateAsync(video4);

            // Act
            var result = unitOfWork.Videos.SearchByString("searchStr", 0, 10);

            // Assert
            long count = result.Count();
            Assert.AreEqual(count, 3);
            videos.DeleteOne(a => a.Id == video1.Id);
            videos.DeleteOne(a => a.Id == video2.Id);
            videos.DeleteOne(a => a.Id == video3.Id);
            videos.DeleteOne(a => a.Id == video4.Id);

            await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
        }
    }
}

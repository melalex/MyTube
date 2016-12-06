using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using MyTube.DAL.Repositories;
using MongoDB.Driver;
using MyTube.DAL.Entities;
using System.Collections.Generic;
using System.Configuration;
using MyTube.DAL.Extensions;
using System.Linq;

namespace MyTube.Tests.MyTube.DAL.Extensions
{
    [TestClass]
    public class HistoryRepositoryExtensionTest
    {
        private MongoClient client;

        public HistoryRepositoryExtensionTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task HistoryRepositoryExtension__GetHistoryFromChannelAsync__Got()
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
            await unitOfWork.Videos.CreateAsync(video1);
            ViewedVideoTransfer transfer = new ViewedVideoTransfer
            {
                ShowDateTime = DateTimeOffset.Now,
                Status = ViewStatus.LIKE,
                Viewer = channel1.DBRef,
                ViewedVideo = video1.DBRef,
            };
            await unitOfWork.ViewedVideoTransfers.CreateAsync(transfer);

            try
            {
                // Act
                var result = await unitOfWork.ViewedVideoTransfers.GetHistoryFromChannelAsync(channel1, 0, 20);

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
                unitOfWork.ViewedVideoTransfers.Collection.DeleteOne(a => a.Id == transfer.Id);
                videos.DeleteOne(a => a.Id == video1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
            }
        }

        [TestMethod]
        public async Task HistoryRepositoryExtension__GetByChannelVideoAsync__Got()
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
            await unitOfWork.Videos.CreateAsync(video1);
            ViewedVideoTransfer transfer = new ViewedVideoTransfer
            {
                ShowDateTime = DateTimeOffset.Now,
                Status = ViewStatus.LIKE,
                Viewer = channel1.DBRef,
                ViewedVideo = video1.DBRef,
            };
            await unitOfWork.ViewedVideoTransfers.CreateAsync(transfer);

            try
            {
                // Act
                var result = await unitOfWork.ViewedVideoTransfers.GetByChannelVideoAsync(channel1.IdString, video1.IdString);

                // Assert
                Assert.IsNotNull(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.ViewedVideoTransfers.Collection.DeleteOne(a => a.Id == transfer.Id);
                videos.DeleteOne(a => a.Id == video1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
            }
        }

        [TestMethod]
        public async Task HistoryRepositoryExtension__GetByChannelVideoAsync__Null()
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
            await unitOfWork.Videos.CreateAsync(video1);

            try
            {
                // Act
                var result = await unitOfWork.ViewedVideoTransfers.GetByChannelVideoAsync(channel1.IdString, video1.IdString);

                // Assert
                Assert.IsNull(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                videos.DeleteOne(a => a.Id == video1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
            }
        }
    }
}

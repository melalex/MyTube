using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MyTube.DAL.Repositories;
using MyTube.DAL.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using MyTube.DAL.Extensions;
using System.Linq;

namespace MyTube.Tests.MyTube.DAL.Extensions
{
    [TestClass]
    public class CommentRepositoryExtensionTest
    {
        private MongoClient client;

        public CommentRepositoryExtensionTest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
        }

        [TestMethod]
        public async Task CommentRepositoryExtension__GetCommentsFromVideo__Got()
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
                Uploader = new MongoDBRef("Channels", channel1.Id),
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

            Comment comment1 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment1",
            };
            Comment comment2 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment2",
            };
            Comment comment3 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment3",
            };
            await unitOfWork.Comments.CreateAsync(comment1);
            await unitOfWork.Comments.CreateAsync(comment2);
            await unitOfWork.Comments.CreateAsync(comment3);


            try
            {
                // Act
                var result = await unitOfWork.Comments.GetCommentsFromVideoAsync(video1.IdString, 0, 20);

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
                unitOfWork.Comments.Collection.DeleteOne(a => a.Id == comment1.Id);
                unitOfWork.Comments.Collection.DeleteOne(a => a.Id == comment2.Id);
                unitOfWork.Comments.Collection.DeleteOne(a => a.Id == comment3.Id);
                videos.DeleteOne(a => a.Id == video1.Id);

                await unitOfWork.Channels.DeleteAsync(channel1.Id.ToString());
            }
        }

        public async Task CommentRepositoryExtension__DeleteCommentsFromVideo__Deleted()
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
                Uploader = new MongoDBRef("Channels", channel1.Id),
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
            Comment comment1 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment1",
            };
            Comment comment2 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment2",
            };
            Comment comment3 = new Comment
            {
                Comentator = channel1.DBRef,
                DestinationVideo = video1.DBRef,
                CommentDateTime = DateTimeOffset.Now,
                Text = "comment3",
            };
            await unitOfWork.Comments.CreateAsync(comment1);
            await unitOfWork.Comments.CreateAsync(comment2);
            await unitOfWork.Comments.CreateAsync(comment3);

            try
            {
                // Act
                await unitOfWork.Comments.DeleteCommentsFromVideoAsync(video1.IdString);
                var result = await unitOfWork.Comments.GetCommentsFromVideoAsync(video1.IdString, 0, 20);

                // Assert
                long count = result.Count();
                Assert.AreEqual(count, 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                await unitOfWork.Channels.DeleteAsync(channel1.IdString);
                await unitOfWork.Videos.DeleteAsync(video1.IdString);
            }
        }
    }
}

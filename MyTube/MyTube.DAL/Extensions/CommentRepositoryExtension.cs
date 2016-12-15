using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Extensions
{
    public static class CommentRepositoryExtension
    {
        public static async Task<IEnumerable<Comment>> GetCommentsFromVideoAsync(
            this IRepositotory<Comment> comments, string videoId, int skip, int limit
            )
        {
            var filter = Builders<Comment>.Filter.Eq(
                c => c.DestinationVideo, new MongoDBRef(Video.collectionName, new ObjectId(videoId))
                );
            var sort = Builders<Comment>.Sort.Descending(c => c.CommentDateTime);
            var options = new FindOptions<Comment>
            {
                Limit = limit,
                Skip = skip,
                Sort = sort,
            };
            var task = await comments.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<long> CommentsCountFromVideoAsync(this IRepositotory<Comment> comments, string videoId)
        {
            var filter = Builders<Comment>.Filter.Eq(
                c => c.DestinationVideo, new MongoDBRef(Video.collectionName, new ObjectId(videoId))
                );
            return await comments.Collection.Find(filter).CountAsync();
        }


        public static async Task DeleteCommentsFromVideoAsync(this IRepositotory<Comment> comments, string videoId)
        {
            var filter = Builders<Comment>.Filter.Eq(
                c => c.DestinationVideo, new MongoDBRef(Video.collectionName, new ObjectId(videoId))
                );
            await comments.Collection.DeleteManyAsync(filter);
        }
    }
}

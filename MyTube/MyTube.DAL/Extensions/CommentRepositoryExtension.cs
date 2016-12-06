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
            this IRepositotory<Comment> comments, Video video, int skip, int limit
            )
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.DestinationVideo, video.DBRef);
            var options = new FindOptions<Comment>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await comments.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task DeleteCommentsFromVideoAsync(this IRepositotory<Comment> comments, Video video)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.DestinationVideo, video.DBRef);
            await comments.Collection.DeleteManyAsync(filter);
        }
    }
}

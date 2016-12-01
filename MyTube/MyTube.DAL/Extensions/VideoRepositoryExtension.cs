using MongoDB.Bson;
using MongoDB.Driver;
using MyTube.DAL.Entities;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Extensions
{
    public static class VideoRepositoryExtension
    {
        public static IEnumerable<Video> SearchByString(this IRepositotory<Video> video, string searchString, int skip, int limit)
        {
            var filter = Builders<Video>.Filter.Text(searchString);
            return video.Collection.Find(filter).Skip(skip).Limit(limit).ToEnumerable();
        }
    }
}

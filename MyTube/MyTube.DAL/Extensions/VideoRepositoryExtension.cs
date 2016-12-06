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
    public static class VideoRepositoryExtension
    {
        public static async Task<IEnumerable<Video>> SearchByStringAsync(
            this IRepositotory<Video> video, string searchString, int skip, int limit
            )
        {
            var filter = Builders<Video>.Filter.Text(searchString);
            var options = new FindOptions<Video>
            {
                Limit = limit,
                Skip = skip,
            };
            var task = await video.Collection.FindAsync(filter, options);
            return await task.ToListAsync();
        }

        public static async Task<IEnumerable<Video>> SearchBYTagsAsync(
            this IRepositotory<Video> videos, List<string> tags, int skip, int limit
            )
        {
            return await videos.Collection
                .Find(v => v.Tags.Any(tag => tags.Contains(tag)))
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        public static async Task<IEnumerable<Video>> GetPopularVideosAsync(
            this IRepositotory<Video> videos, int skip, int limit
            )
        {
            Func<int, int, double> wilsonScore = (likes, dislikes) =>
            {
                int n = likes + dislikes;
                if (n == 0)
                    return 0;
                double z = 1.96;
                double phat = 1.0 * likes / n;
                return (phat + z * z / (2 * n) - z * Math.Sqrt((phat * (1 - phat) + z * z / (4 * n)) / n)) / (1 + z * z / n);
            };
            return await videos
                .Collection
                .AsQueryable()
                .OrderBy(v => wilsonScore(v.Likes, v.Dislikes))
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
        }
    }
}

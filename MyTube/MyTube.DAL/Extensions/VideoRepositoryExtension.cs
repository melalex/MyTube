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
        public static async Task<IEnumerable<Video>> SearchByString(this IRepositotory<Video> video, string searchString, int skip, int limit)
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

        public static async Task<IEnumerable<Video>> SearchBYTags(this IRepositotory<Video> videos, List<string> tags, int skip, int limit)
        {          
                var similarVideos = from v in videos.Collection.AsQueryable()
                                    let intersection = v.Tags.Intersect(tags)
                                    where intersection.Any()
                                    select new {
                                        IntersectionCount = intersection.Count(),
                                        SimilarVideo = v,
                                    };
                return await similarVideos
                .OrderByDescending(x => x.IntersectionCount)
                .Skip(skip)
                .Take(limit)
                .Select(x => x.SimilarVideo)
                .ToListAsync();
        }

        public static async Task<IEnumerable<Video>> GetPopularVideos(this IRepositotory<Video> videos, int skip, int limit)
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

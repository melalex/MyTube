using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models.Caching
{
    public static class CacheKeys
    {
        public static string ChannelCacheKey(string userId)
        {
            return $"channel:{userId}";
        }

        public static string EditChannelCacheKey(string userId)
        {
            return $"channel:edit:{userId}";
        }

        public static string ChannelThumbnailCacheKey(string userId)
        {
            return $"channel:thumbnail:{userId}";
        }

        public static string ChannelVideosCacheKey(string userId)
        {
            return $"channel:videos:{userId}";
        }

        public static string VideoCacheKey(string videoId)
        {
            return $"video:{videoId}";
        }


        public static string VideoCommentsCacheKey(string videoId)
        {
            return $"video:comments:{videoId}";
        }

        public static string PopularVideosCacheKey()
        {
            return "videos:popular";
        }

        public static string PopularVideosWithPaginationCacheKey()
        {
            return "videos:popular:pagination";
        }

        public static string SimilarVideosCacheKey()
        {
            return $"videos:similar";
        }

        public static string SubscriptionCacheKey(string userId)
        {
            return $"subscription:{userId}";
        }

        public static string SubscribersCountCacheKey(string userId)
        {
            return $"subscribers:count:{userId}";
        }

        public static string IsSubscriberCacheKey(string publisher, string subscriber)
        {
            return $"subscribers:{publisher}:{subscriber}";
        }

        public static string FulltextSearchCacheKey()
        {
            return "search:fulltext";
        }

        public static string CategorySearchCacheKey(string parameter)
        {
            return $"search:category:{parameter}";
        }

        public static string TagsSearchCacheKey(string parameter)
        {
            return $"search:tags:{parameter}";
        }
    }
}
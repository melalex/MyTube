using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace MyTube.WEB.Models.Caching
{
    public static class Redis
    {
        private static string logSetKey = "log:keys";

        public static string AspNetCacheReadsCountKey { get; private set; } = "asp";
        public static string RedisReadsCountKey { get; private set; } = "redis";
        public static string MongoReadsCountKey { get; private set; } = "mongo";
        
        public static readonly ConnectionMultiplexer Client = ConnectionMultiplexer.Connect("localhost");

        public static async Task<Dictionary<string, Dictionary<string, long>>> CacheLog()
        {
            var result = new Dictionary<string, Dictionary<string, long>>();

            var redisDb = Client.GetDatabase();
            var logKeys = await redisDb.SetMembersAsync(logSetKey);

            string keyString = null;
            long value = 0;

            foreach (var key in logKeys)
            {
                keyString = key.ToString();
                result[keyString] = new Dictionary<string, long>();

                (await redisDb.HashGetAsync(keyString, AspNetCacheReadsCountKey)).TryParse(out value);
                result[keyString][AspNetCacheReadsCountKey] = value;

                (await redisDb.HashGetAsync(keyString, RedisReadsCountKey)).TryParse(out value);
                result[keyString][RedisReadsCountKey] = value;

                (await redisDb.HashGetAsync(keyString, MongoReadsCountKey)).TryParse(out value);
                result[keyString][MongoReadsCountKey] = value;
            }

            return result;
        }

        public static CacheDependency CreateDependency(string key)
        {
            return new RedisCacheDependency(key);
        }

        public static T GetCached<T>(string key, Func<T> getter) where T : class
        {
            string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            string logKey = $"log:{controllerName}:{actionName}";

            var redisDb = Client.GetDatabase();

            redisDb.SetAdd(logSetKey, logKey);

            var localCache = HttpRuntime.Cache;
            var result = (T)localCache.Get(key);

            if (result != null)
            {
                redisDb.HashIncrement(logKey, AspNetCacheReadsCountKey);
                return result;
            }


            var value = redisDb.StringGet(key);
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);
                localCache.Insert(key, result, CreateDependency(key));
                redisDb.HashIncrement(logKey, RedisReadsCountKey);
                return result;
            }

            result = getter();

            redisDb.StringSet(key, JsonConvert.SerializeObject(result));
            localCache.Insert(key, result, CreateDependency(key));
            redisDb.HashIncrement(logKey, MongoReadsCountKey);
            return result;
        }

        public async static Task<T> GetCachedAsync<T>(string key, Func<Task<T>> getter) where T : class
        {
            string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            string logKey = $"log:{controllerName}:{actionName}";

            var redisDb = Client.GetDatabase();

            await redisDb.SetAddAsync(logSetKey, logKey);

            var localCache = HttpRuntime.Cache;
            var result = (T)localCache.Get(key);

            if (result != null)
            {
                await redisDb.HashIncrementAsync(logKey, AspNetCacheReadsCountKey);
                return result;
            }

            var value = await redisDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);
                localCache.Insert(key, result, CreateDependency(key));
                await redisDb.HashIncrementAsync(logKey, RedisReadsCountKey);
                return result;
            }

            result = await getter();

            await redisDb.StringSetAsync(key, JsonConvert.SerializeObject(result));
            localCache.Insert(key, result, CreateDependency(key));
            await redisDb.HashIncrementAsync(logKey, MongoReadsCountKey);
            return result;
        }

        public async static Task<T> GetCachedPageAsync<T>(string key, int page, Func<Task<T>> getter) where T : class
        {
            string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            string logKey = $"log:{controllerName}:{actionName}";

            var redisDb = Client.GetDatabase();

            await redisDb.SetAddAsync(logSetKey, logKey);

            var localCache = HttpRuntime.Cache;
            T result = null;

            var pages = (Dictionary<int, T>)localCache.Get(key);
            if (pages != null)
            {
                bool isGeted = pages.TryGetValue(page, out result);
                if (isGeted)
                {
                    await redisDb.HashIncrementAsync(logKey, AspNetCacheReadsCountKey);
                    return result;
                }
            }
            else
            {
                pages = new Dictionary<int, T>();
            }

            
            var value = await redisDb.HashGetAsync(key, page);
            
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);
                pages[page] = result;
                localCache.Insert(key, pages, CreateDependency(key));
                await redisDb.HashIncrementAsync(logKey, RedisReadsCountKey);
                return result;
            }

            result = await getter();
            pages[page] = result;

            await redisDb.HashSetAsync(key, page, JsonConvert.SerializeObject(result));
            localCache.Insert(key, pages, CreateDependency(key));
            await redisDb.HashIncrementAsync(logKey, MongoReadsCountKey);
            return result;
        }

        public async static Task<T> GetCachedWithParamAsync<T>(string key, string parameter, Func<Task<T>> getter) where T : class
        {
            string controllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string actionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            string logKey = $"log:{controllerName}:{actionName}";

            var redisDb = Client.GetDatabase();

            await redisDb.SetAddAsync(logSetKey, logKey);

            var localCache = HttpRuntime.Cache;
            T result = null;

            var pages = (Dictionary<string, T>)localCache.Get(key);
            if (pages != null)
            {
                bool isGeted = pages.TryGetValue(parameter, out result);
                if (isGeted)
                {
                    await redisDb.HashIncrementAsync(logKey, AspNetCacheReadsCountKey);
                    return result;
                }
            }
            else
            {
                pages = new Dictionary<string, T>();
            }


            var value = await redisDb.HashGetAsync(key, parameter);
            if (!value.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(value);
                pages[parameter] = result;
                localCache.Insert(key, pages, CreateDependency(key));
                await redisDb.HashIncrementAsync(logKey, RedisReadsCountKey);
                return result;
            }

            result = await getter();
            pages[parameter] = result;

            await redisDb.HashSetAsync(key, parameter, JsonConvert.SerializeObject(result));
            localCache.Insert(key, pages, CreateDependency(key));
            await redisDb.HashIncrementAsync(logKey, MongoReadsCountKey);
            return result;
        }

        public async static Task UpdateCacheAsync<T>(string key, T target) where T : class
        {
            HttpRuntime.Cache[key] = target;
            var jsonTarget = JsonConvert.SerializeObject(target);
            var redisDb = Client.GetDatabase();
            await redisDb.StringSetAsync(key, jsonTarget);
        }

        public async static Task DeleteKeyAsync(string key)
        {
            HttpRuntime.Cache.Remove(key);
            var redisDb = Client.GetDatabase();
            await redisDb.KeyDeleteAsync(key);
        }
    }
}
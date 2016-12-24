using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;

namespace MyTube.WEB.Models.Caching
{
    public static class Redis
    {
        public static readonly ConnectionMultiplexer Client = ConnectionMultiplexer.Connect("localhost");

        public static CacheDependency CreateDependency(string key)
        {
            return new RedisCacheDependency(key);
        }

        public async static Task<T> GetCachedAsync<T>(string key, Func<T> getter) where T : class
        {
            var localCache = HttpRuntime.Cache;
            var result = (T)localCache.Get(key);
            if (result != null) return result;

            var redisDb = Client.GetDatabase();

            var value = await redisDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                result = Json.Decode<T>(value);
                localCache.Insert(key, result, CreateDependency(key));
                return result;
            }

            result = getter();

            await redisDb.StringSetAsync(key, Json.Encode(result));
            localCache.Insert(key, result, CreateDependency(key));
            return result;
        }

        public async static Task<T> GetCachedAsync<T>(string key, Func<Task<T>> getter) where T : class
        {
            var localCache = HttpRuntime.Cache;
            var result = (T)localCache.Get(key);
            if (result != null) return result;

            var redisDb = Client.GetDatabase();

            var value = await redisDb.StringGetAsync(key);
            if (!value.IsNullOrEmpty)
            {
                result = Json.Decode<T>(value);
                localCache.Insert(key, result, CreateDependency(key));
                return result;
            }

            result = await getter();

            await redisDb.StringSetAsync(key, Json.Encode(result));
            localCache.Insert(key, result, CreateDependency(key));
            return result;
        }

        public async static Task<T> GetCachedPageAsync<T>(string key, int page, Func<Task<T>> getter) where T : class
        {
            var localCache = HttpRuntime.Cache;
            var redisDb = Client.GetDatabase();
            T result = null;

            var pages = (Dictionary<int, T>)localCache.Get(key);
            if (pages != null)
            {
                bool isGeted = pages.TryGetValue(page, out result);
                if (isGeted)
                {
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
                result = Json.Decode<T>(value);
                pages[page] = result;
                localCache.Insert(key, pages, CreateDependency(key));
                return result;
            }

            result = await getter();
            pages[page] = result;

            await redisDb.HashSetAsync(key, page, Json.Encode(result));
            localCache.Insert(key, pages, CreateDependency(key));
            return result;
        }

        public async static Task<T> GetCachedWithParamAsync<T>(string key, string parameter, Func<Task<T>> getter) where T : class
        {
            var localCache = HttpRuntime.Cache;
            var redisDb = Client.GetDatabase();
            T result = null;

            var pages = (Dictionary<string, T>)localCache.Get(key);
            if (pages != null)
            {
                bool isGeted = pages.TryGetValue(parameter, out result);
                if (isGeted)
                {
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
                result = Json.Decode<T>(value);
                pages[parameter] = result;
                localCache.Insert(key, pages, CreateDependency(key));
                return result;
            }

            result = await getter();
            pages[parameter] = result;

            await redisDb.HashSetAsync(key, parameter, Json.Encode(result));
            localCache.Insert(key, pages, CreateDependency(key));
            return result;
        }

        public async static Task UpdateCacheAsync<T>(string key, T target) where T : class
        {
            HttpRuntime.Cache[key] = target;
            var jsonTarget = Json.Encode(target);
            var redisDb = Client.GetDatabase();
            await redisDb.StringSetAsync(key, Json.Encode(jsonTarget));
        }

        public async static Task DeleteKeyAsync(string key)
        {
            HttpRuntime.Cache.Remove(key);
            var redisDb = Client.GetDatabase();
            await redisDb.KeyDeleteAsync(key);
        }
    }
}
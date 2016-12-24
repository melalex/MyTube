using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models.Caching
{
    public static class CacheHelper
    {
        public static void SetCache(
            this HttpResponseBase response, 
            string cacheKey, 
            bool isUserDepent = false, 
            params string[] varyParams
            )
        {
            response.AddCacheItemDependency(cacheKey);
            response.Cache.SetLastModifiedFromFileDependencies();
            response.Cache.AppendCacheExtension("max-age=0");

            varyParams.ToList().ForEach(p => response.Cache.VaryByParams[p] = true);

            if (isUserDepent)
            {
                response.Cache.SetVaryByCustom("User");
            }

            response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
        }
    }
}
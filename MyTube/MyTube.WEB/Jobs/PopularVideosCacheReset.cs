using MyTube.WEB.Models.Caching;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Jobs
{
    public class PopularVideosCacheReset : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Redis.DeleteKeyAsync(CacheKeys.PopularVideosCacheKey());
            Redis.DeleteKeyAsync(CacheKeys.PopularVideosWithPaginationCacheKey());
        }
    }
}
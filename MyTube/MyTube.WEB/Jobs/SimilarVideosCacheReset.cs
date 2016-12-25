using MyTube.WEB.Models.Caching;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Jobs
{
    public class SimilarVideosCacheReset : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            string key = CacheKeys.SimilarVideosCacheKey();
            Redis.DeleteKeyAsync(key);
        }
    }
}
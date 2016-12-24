using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MyTube.WEB.Models.Caching
{
    public class RedisCacheDependency : CacheDependency
    {
        public RedisCacheDependency(string key) : base()
        {
            Redis.Client.GetSubscriber().Subscribe(key, (c, v) =>
            {
                NotifyDependencyChanged(new object(), EventArgs.Empty);
            });
        }
    }
}
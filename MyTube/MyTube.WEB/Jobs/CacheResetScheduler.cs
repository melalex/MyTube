using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Jobs
{
    public static class CacheResetScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail popularVideosCacheResetJob = JobBuilder.Create<PopularVideosCacheReset>().Build();
            IJobDetail similarVideosCacheResetJob = JobBuilder.Create<SimilarVideosCacheReset>().Build();

            ITrigger popularVideosCacheResetTrigger = TriggerBuilder.Create()
                .WithIdentity("popularVideosCacheResetTrigger", "CacheReset")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            ITrigger similarVideosCacheResetTrigger = TriggerBuilder.Create()
                .WithIdentity("similarVideosCacheResetTrigger", "CacheReset")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(popularVideosCacheResetJob, popularVideosCacheResetTrigger);
            scheduler.ScheduleJob(similarVideosCacheResetJob, similarVideosCacheResetTrigger);
        }
    }
}
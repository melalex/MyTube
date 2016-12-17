using MyTube.BLL.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models.Channel
{
    public class ChannelThumbnailViewModel
    {
        public ChannelProxy Channel { get; set; }
        public long VideosCount { get; set; }
        public long SubscribersCount { get; set; }
    }
}
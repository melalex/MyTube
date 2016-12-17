using MyTube.BLL.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models.Channel
{
    public class ChannelViewModel
    {
        public ChannelProxy Channel { get; set; }
        public bool IsSubscriber { get; set; }
        public bool IsMyChannel { get; set; }
        public long SubscribersCount { get; set; }
    }
}
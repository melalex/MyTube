using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.BLL.DTO;
using MyTube.DAL.Interfaces;
using MyTube.DAL.Extensions;
using MyTube.BLL.Interfaces;
using AutoMapper;
using MyTube.DAL.Entities;

namespace MyTube.BLL.BusinessEntities
{
    public class ChannelProxy
    {
        public ChannelProxy()
        {
            channel = new Channel();
        }

        public ChannelProxy(Channel channel)
        {
            this.channel = channel;
        }

        internal Channel channel { get; private set; }

        public string Id
        {
            get
            {
                return channel.IdString;
            }
            set
            {
                channel.IdString = value;
            }
        }
        public string Username
        {
            get
            {
                return channel.Username;
            }
            set
            {
                channel.Username = value;
            }
        }
        public string AvatarUri
        {
            get
            {
                return channel.AvatarUri;
            }
            set
            {
                channel.AvatarUri = value;
            }
        }
    }
}

﻿using System;
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
        private IUnitOfWork database;

        public ChannelProxy(IUnitOfWork database, DAL.Entities.Channel channel)
        {
            this.database = database;
            this.channel = channel;
        }

        internal DAL.Entities.Channel channel { get; private set; }

        public string Id
        {
            get
            {
                return channel.IDString;
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

        public async Task<IEnumerable<SubscriptionDTO>> Subscribers(int skip, int limit)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Subscription, SubscriptionDTO>()
                    .ForMember(s => s.Id, s => s.MapFrom(scr => scr.IDString))
                    .ForMember(s => s.SubscriptionDate, s => s.MapFrom(scr => scr.StartDate)));

            var result = await database.Subscriptions.GetSubscribers(channel, skip, limit);
            return Mapper.Map<IEnumerable<Subscription>, List<SubscriptionDTO>>(result);
        }

        public async Task<IEnumerable<SubscriptionDTO>> Subscriptions(int skip, int limit)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Subscription, SubscriptionDTO>()
                    .ForMember(s => s.Id, s => s.MapFrom(scr => scr.IDString))
                    .ForMember(s => s.SubscriptionDate, s => s.MapFrom(scr => scr.StartDate)));

            var result = await database.Subscriptions.GetSubscribtions(channel, skip, limit);
            return Mapper.Map<IEnumerable<Subscription>, List<SubscriptionDTO>>(result);
        }

        public async Task<IEnumerable<NotificationDTO>> Notifications(int skip, int limit)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Notification, NotificationDTO>()
                    .ForMember(s => s.Id, s => s.MapFrom(scr => scr.IDString)));

            var result = await database.Notifications.GetNotificationFromChannel(channel, skip, limit);
            return Mapper.Map<IEnumerable<Notification>, List<NotificationDTO>>(result);
        }

        public async Task<IEnumerable<ViewedVideoTransferDTO>> History(int skip, int limit)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<ViewedVideoTransfer, ViewedVideoTransferDTO>()
                    .ForMember(s => s.Id, s => s.MapFrom(scr => scr.IDString))
                    .ForMember(s => s.Viewer, s => s.MapFrom(scr => scr.ViewerIdString))
                    .ForMember(s => s.ViewedVideo, s => s.MapFrom(scr => scr.ViewedVideoIdString))
                    .ForMember(s => s.Status, s => s.MapFrom(scr => (Interfaces.ViewStatus)scr.Status)));
    
            var result = await database.ViewedVideoTransfers.GetNotificationFromChannel(channel, skip, limit);
            return Mapper.Map<IEnumerable<ViewedVideoTransfer>, List<ViewedVideoTransferDTO>>(result);
        }
    }
}

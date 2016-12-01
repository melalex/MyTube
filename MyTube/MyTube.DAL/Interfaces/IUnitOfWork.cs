using MyTube.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepositotory<Channel> Channels { get; }
        IRepositotory<Video> Videos { get; }
        IRepositotory<Comment> Comments { get; }
        IRepositotory<Notification> Notifications { get; }
        IRepositotory<Subscription> Subscriptions { get; }
        IRepositotory<ViewedVideoTransfer> ViewedVideoTransfers { get; }
    }
}

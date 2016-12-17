using Microsoft.AspNet.Identity;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
using MyTube.BLL.Interfaces;
using MyTube.WEB.Models.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Controllers
{
    public class ChannelController : Controller
    {
        private const int videosOnPage = 20;

        private IUserService userService;

        public ChannelController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: Channel/ChannelProfile/id
        public async Task<ActionResult> ChannelProfile(string id)
        {
            string currentUser = User.Identity.GetUserId();
            ChannelViewModel channel = new ChannelViewModel
            {
                Channel = await userService.GetChannelAsync(id),
                SubscribersCount = await userService.SubscribersCountAsync(id),
                IsSubscriber = await userService.IsSubscriberAsync(id, currentUser),
                IsMyChannel = id == currentUser,
            };
            return View(channel);
        }

        // GET: Channel/Videos/page
        public async Task<ActionResult> Videos(string parametr, int page)
        {
            var videos = await userService.GetVideosFromChannelAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
            long videosCount = await userService.GetVideosFromChannelCountAsync(parametr);
            ViewBag.CommentCount = videosCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * videosCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Videos";
            ViewBag.Controller = "Channel";
            ViewBag.Parametr = parametr;
            return PartialView(videos);
        }

        // GET: Channel/Subscriptions/page
        [Authorize]
        public async Task<ActionResult> Subscriptions(int page = 1)
        {
            string currentUser = User.Identity.GetUserId();
            var subscriptions = await userService.SubscriptionsAsync(currentUser, (page - 1) * videosOnPage, videosOnPage);

            var tasks = subscriptions.Select(async s => 
            {
                return new ChannelThumbnailViewModel
                {
                    Channel = s,
                    SubscribersCount = await userService.SubscribersCountAsync(s.Id),
                    VideosCount = await userService.GetVideosFromChannelCountAsync(s.Id),
                };
            }).ToList();
            var subscriptionsViewModels = await Task.WhenAll(tasks);

            long subscriptionsCount = await userService.SubscriptionsCountAsync(currentUser);
            ViewBag.SubscriptionsCount = subscriptionsCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * subscriptionsCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Subscriptions";
            ViewBag.Controller = "Channel";
            ViewBag.Parametr = null;
            return View(subscriptions);
        }

        // POST: Channel/Subscribe/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Subscribe(string id)
        {
            SubscriptionDTO subscription = new SubscriptionDTO
            {
                StartDate = DateTimeOffset.Now,
                Publisher = id,
                Subscriber = User.Identity.GetUserId(),
            };
            await userService.SubscribeAsync(subscription);
            return Json(new { status = "OK" });
        }

        // POST: Channel/Unsubscribe/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unsubscribe(string id)
        {
            await userService.UnsubscribeAsync(id, User.Identity.GetUserId());
            return Json(new { status = "OK" });
        }
    }
}
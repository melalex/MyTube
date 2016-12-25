using Microsoft.AspNet.Identity;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
using MyTube.BLL.Interfaces;
using MyTube.WEB.Models.Caching;
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
            string channelKey = CacheKeys.ChannelCacheKey(id);

            var channelProxy = await Redis.GetCachedAsync(
                channelKey, async () => await userService.GetChannelAsync(id)
                );

            ChannelViewModel channel = new ChannelViewModel
            {
                Channel = channelProxy,
                SubscribersCount = await userService.SubscribersCountAsync(id),
                IsSubscriber = await userService.IsSubscriberAsync(id, currentUser),
                IsMyChannel = id == currentUser,
            };
            
            return View(channel);
        }

        // GET: Channel/Thumbnail/id
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Thumbnail(string id)
        {
            string key = CacheKeys.ChannelThumbnailCacheKey(id);

            ChannelThumbnailViewModel channel = Redis.GetCached(key, () => 
            {
                return new ChannelThumbnailViewModel
                {
                    Channel = Task.Run(() => Redis.GetCachedAsync(
                        CacheKeys.ChannelCacheKey(id), async () => await userService.GetChannelAsync(id)
                        )).Result,
                    SubscribersCount = Task.Run(() => userService.SubscribersCountAsync(id)).Result,
                    VideosCount = Task.Run(() => userService.GetVideosFromChannelCountAsync(id)).Result,
                };
            });

            Response.SetCache(key, false, "id");

            return PartialView("_ChannelThumbnail", channel);
        }

        // GET: Channel/Videos/page
        public async Task<ActionResult> Videos(string parametr, int page)
        {
            string key = CacheKeys.ChannelVideosCacheKey(parametr);

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () => 
                {
                    var videos = await userService.GetVideosFromChannelAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
                    long videosCount = await userService.GetVideosFromChannelCountAsync(parametr);
                    return new Tuple<IEnumerable<string>, long>(videos.Select(v => v.Id), videosCount);
                });

            ViewBag.CommentCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Videos";
            ViewBag.Controller = "Channel";
            ViewBag.Parametr = parametr;

            Response.SetCache(key, false, "parametr", "page");

            return PartialView(tuple.Item1);
        }

        // GET: Channel/Subscriptions/page
        [Authorize]
        public async Task<ActionResult> Subscriptions(int page = 1)
        {
            string currentUser = User.Identity.GetUserId();
            string key = CacheKeys.SubscriptionCacheKey(currentUser);

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () => {
                    var subscriptions = await userService.SubscriptionsAsync(currentUser, (page - 1) * videosOnPage, videosOnPage);
                    long subscriptionsCount = await userService.SubscriptionsCountAsync(currentUser);
                    return new Tuple<IEnumerable<string>, long>(subscriptions.Select(v => v.Id), subscriptionsCount);
                });
                 
            ViewBag.SubscriptionsCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Subscriptions";
            ViewBag.Controller = "Channel";
            ViewBag.Parametr = null;

            Response.SetCache(key, true);

            return View(tuple.Item1);
        }

        // POST: Channel/Subscribe/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Subscribe(string id)
        {
            string currentUser = User.Identity.GetUserId();
            string subscriptionKey = CacheKeys.SubscriptionCacheKey(currentUser);
            string channelKey = CacheKeys.ChannelThumbnailCacheKey(id);

            SubscriptionDTO subscription = new SubscriptionDTO
            {
                StartDate = DateTimeOffset.Now,
                Publisher = id,
                Subscriber = currentUser,
            };
            await userService.SubscribeAsync(subscription);

            await Redis.DeleteKeyAsync(subscriptionKey);
            await Redis.DeleteKeyAsync(channelKey);

            return Json(new { status = "OK" });
        }

        // POST: Channel/Unsubscribe/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unsubscribe(string id)
        {
            string currentUser = User.Identity.GetUserId();
            string subscriptionKey = CacheKeys.SubscriptionCacheKey(currentUser);
            string channelKey = CacheKeys.ChannelThumbnailCacheKey(id);

            await userService.UnsubscribeAsync(id, currentUser);

            await Redis.DeleteKeyAsync(subscriptionKey);
            await Redis.DeleteKeyAsync(channelKey);

            return Json(new { status = "OK" });
        }
    }
}
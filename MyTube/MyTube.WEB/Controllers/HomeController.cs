using MyTube.BLL.Interfaces;
using MyTube.WEB.Models.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Controllers
{
    public class HomeController : Controller
    {
        private const int videosOnPage = 20;

        private IUserService userService;

        public HomeController(IUserService userService)
        {
            this.userService = userService;
        }
         
        public async Task<ActionResult> Index()
        {
            string key = CacheKeys.PopularVideosCacheKey();
            var popular = await Redis.GetCachedPageAsync(
                key, 1, async () =>
                {
                    var result = await userService.GetPopularVideosAsync(0, videosOnPage);
                    return result.Select(v => v.Id);
                });
            Response.SetCache(key);
            return View(popular);
        }
    }
}
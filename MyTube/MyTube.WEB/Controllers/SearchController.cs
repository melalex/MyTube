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
    public class SearchController : Controller
    {
        private const int videosOnPage = 20;

        private IUserService userService;

        public SearchController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: Search/Fulltext/page
        public async Task<ActionResult> Fulltext(string parametr, int page = 1)
        {
            string key = CacheKeys.FulltextSearchCacheKey();

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedWithParamAsync(
                key, parametr, async () =>
                {
                    var videos = await userService.FulltextSearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
                    long videosCount = await userService.FulltextCountSearchAsync(parametr);
                    return new Tuple<IEnumerable<string>, long>(videos.Select(v => v.Id), videosCount);
                });

            ViewBag.VideoCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Fulltext";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;
            ViewBag.SearchString = parametr;

            Response.SetCache(key, false, "parametr", "page");

            return View("Search", tuple.Item1);
        }

        // GET: Search/Category/page
        public async Task<ActionResult> Category(string parametr, int page)
        {
            string key = CacheKeys.CategorySearchCacheKey(parametr);

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () =>
                {
                    var videos = await userService.CategorySearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
                    long videosCount = await userService.CategorySearchCountAsync(parametr);
                    return new Tuple<IEnumerable<string>, long>(videos.Select(v => v.Id), videosCount);
                });
             
            ViewBag.VideoCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Category";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;

            Response.SetCache(key, false, "parametr", "page");

            return View("Search", tuple.Item1);
        }

        // GET: Search/Tags/page
        public async Task<ActionResult> Tags(string parametr, int page)
        {
            string key = CacheKeys.TagsSearchCacheKey(parametr);

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () =>
                {
                    var videos = await userService.TagsSearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
                    long videosCount = await userService.TagsSearchCountAsync(parametr);
                    return new Tuple<IEnumerable<string>, long>(videos.Select(v => v.Id), videosCount);
                });
             
            ViewBag.VideoCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Tags";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;

            Response.SetCache(key, false, "parametr", "page");

            return View("Search", tuple.Item1);
        }

        // GET: Search/Popular/page
        public async Task<ActionResult> Popular(int page)
        {
            string key = CacheKeys.PopularVideosCacheKey();

            Tuple<IEnumerable<string>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () =>
                {
                    var videos = await userService.GetPopularVideosAsync((page - 1) * videosOnPage, videosOnPage);
                    long videosCount = await userService.VideosCountAsync();
                    return new Tuple<IEnumerable<string>, long>(videos.Select(v => v.Id), videosCount);
                });

            ViewBag.VideoCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Popular";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = null;

            Response.SetCache(key, false, "page");

            return View("Search", tuple.Item1);
        }
    }
}
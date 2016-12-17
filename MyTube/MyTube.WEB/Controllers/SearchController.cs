using MyTube.BLL.Interfaces;
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
            var videos = await userService.FulltextSearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
            long videosCount = await userService.FulltextCountSearchAsync(parametr);
            ViewBag.VideoCount = videosCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * videosCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Fulltext";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;
            ViewBag.SearchString = parametr;
            return View("Search", videos);
        }

        // GET: Search/Category/page
        public async Task<ActionResult> Category(string parametr, int page)
        {
            var videos = await userService.CategorySearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
            long videosCount = await userService.CategorySearchCountAsync(parametr);
            ViewBag.VideoCount = videosCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * videosCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Category";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;
            return View("Search", videos);
        }

        // GET: Search/Tags/page
        public async Task<ActionResult> Tags(string parametr, int page)
        {
            var videos = await userService.TagsSearchAsync(parametr, (page - 1) * videosOnPage, videosOnPage);
            long videosCount = await userService.TagsSearchCountAsync(parametr);
            ViewBag.VideoCount = videosCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * videosCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Tags";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = parametr;
            return View("Search", videos);
        }

        // GET: Search/Popular/page
        public async Task<ActionResult> Popular(int page)
        {
            var videos = await userService.GetPopularVideosAsync((page - 1) * videosOnPage, videosOnPage);
            long videosCount = await userService.VideosCountAsync();
            ViewBag.VideoCount = videosCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * videosCount / videosOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Popular";
            ViewBag.Controller = "Search";
            ViewBag.Parametr = null;
            return View("Search", videos);
        }
    }
}
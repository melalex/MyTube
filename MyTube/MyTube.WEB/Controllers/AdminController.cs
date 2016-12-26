using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyTube.BLL.Identity.Interfaces;
using MyTube.WEB.Models.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CacheReport()
        {
            var model = await Redis.CacheLog();

            ViewBag.ASP = Redis.AspNetCacheReadsCountKey;
            ViewBag.Redis = Redis.RedisReadsCountKey;
            ViewBag.Mongo = Redis.MongoReadsCountKey;

            return View(model);
        }
    }
}
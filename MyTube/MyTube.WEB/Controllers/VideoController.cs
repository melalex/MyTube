using Microsoft.AspNet.Identity;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.Interfaces;
using MyTube.WEB.Models;
using MyTube.WEB.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Controllers
{
    public class VideoController : Controller
    {
        private const int commentsOnPage = 20;

        private IUserService userService;

        public VideoController(IUserService userService)
        {
            this.userService = userService;
        }

        // GET: Video/UploadVideo
        [HttpGet]
        [Authorize]
        public ActionResult UploadVideo()
        {
            return View(new VideoUploadViewModel());
        }

        // POST: Video/UploadVideo
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVideo(VideoUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                string videoPath = FileUpload.UploadFile(model.Video);
                string posterPath = FileUpload.UploadFile(model.Poster);
                string channelId = User.Identity.GetUserId();

                var createVideoTask = userService.CreateVideoAsync(
                    channelId,
                    model.Name,
                    model.Description,
                    model.SelectedCategory,
                    model.TagsList,
                    videoPath,
                    posterPath
                    );

                createVideoTask.ContinueWith((previos) => {
                    FileUpload.RemoveFile(videoPath);
                    FileUpload.RemoveFile(posterPath);
                });

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Video/Watch/id
        public async Task<ActionResult> Watch(string id)
        {
            VideoProxy video = await userService.GetVideoAsync(id);
            
            if (video == null)
            {
                throw new HttpException(404, "Video does not exist");
            }
            
            return View(video);
        }

        // GET: Video/Similar/id
        public async Task<ActionResult> Similar(string id)
        {
            VideoProxy video = await userService.GetVideoAsync(id);
            var similarVideos = await userService.GetSimilarVideosAsync(video, 0, 10);
            return PartialView(similarVideos);
        }

        public async Task<ActionResult> Comments(string id, int page)
        {
            var comments = await userService.GetCommentsAsync(id, (page - 1) * commentsOnPage, commentsOnPage);
            long commentCount = await userService.GetCommentsCountAsync(id);
            ViewBag.PageCount = Math.Ceiling(1.0 * commentCount / commentsOnPage);
            ViewBag.Page = page;
            return PartialView(comments);
        }
    }
}
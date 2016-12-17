using Microsoft.AspNet.Identity;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
using MyTube.BLL.Interfaces;
using MyTube.WEB.Models;
using MyTube.WEB.Models.Channel;
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
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            VideoProxy video = await userService.GetVideoAsync(id, Request.UserHostAddress);
            
            if (video == null)
            {
                throw new HttpException(404, "Video does not exist");
            }

            string currentUser = User.Identity.GetUserId();
            ViewBag.IsMyChannel = currentUser == video.UploaderId;
            ViewBag.IsSubscriber = await userService.IsSubscriberAsync(id, currentUser);

            return View(video);
        }

        // GET: Video/Similar/id
        public async Task<ActionResult> Similar(string id)
        {
            VideoProxy video = await userService.GetVideoAsync(id);
            var similarVideos = await userService.GetSimilarVideosAsync(video, 0, 10);
            return PartialView(similarVideos);
        }

        // GET: Video/Comments/parametr/page
        public async Task<ActionResult> Comments(string parametr, int page)
        {
            var comments = await userService.GetCommentsAsync(parametr, (page - 1) * commentsOnPage, commentsOnPage);
            long commentCount = await userService.GetCommentsCountAsync(parametr);
            ViewBag.CommentCount = commentCount;
            ViewBag.PageCount = Math.Ceiling(1.0 * commentCount / commentsOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Comments";
            ViewBag.Controller = "Video";
            ViewBag.Parametr = parametr;
            return PartialView(comments);
        }

        // POST: Video/AddComment/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(string id, string comment)
        {
            CommentDTO newComment = new CommentDTO
            {
                CommentatorId = User.Identity.GetUserId(),
                CommentDateTime = DateTimeOffset.Now,
                CommentText = comment,
                VideoId = id,
            };
            await userService.AddCommentAsync(newComment);
            return Json(new { status = "OK" });
        }

        // POST: Video/Estimate/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Estimate(string id, int status)
        {
            ViewedVideoTransferDTO transfer = new ViewedVideoTransferDTO
            {
                Status = (ViewStatus)status,
                ViewedVideo = id,
                Viewer = User.Identity.GetUserId(),
                ShowDateTime = DateTimeOffset.Now,
            };
            userService.EstimateVideoAsync(transfer);
            return Json(new { status = "OK" });
        }
    }
}
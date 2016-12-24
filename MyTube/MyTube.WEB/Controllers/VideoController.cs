using Microsoft.AspNet.Identity;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.DTO;
using MyTube.BLL.Interfaces;
using MyTube.WEB.Models;
using MyTube.WEB.Models.Caching;
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

                Redis.DeleteKeyAsync(CacheKeys.ChannelVideosCacheKey(channelId));
                Redis.DeleteKeyAsync(CacheKeys.FulltextSearchCacheKey());
                Redis.DeleteKeyAsync(CacheKeys.CategorySearchCacheKey(model.SelectedCategory));

                foreach (string tag in model.TagsList)
                {
                    Redis.DeleteKeyAsync(CacheKeys.TagsSearchCacheKey(tag));
                }

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

            string key = CacheKeys.VideoCacheKey(id);

            VideoProxy video = await Redis.GetCachedAsync(
                key, async () => 
                {
                    var videoProxy = await userService.GetVideoAsync(id);
                    if (await userService.AddView(videoProxy, Request.UserHostAddress))
                    {
                        await Redis.UpdateCacheAsync(key, videoProxy);
                    }
                    return videoProxy;
                });
            
            if (video == null)
            {
                throw new HttpException(404, "Video does not exist");
            }

            string currentUser = User.Identity.GetUserId();
            ViewBag.IsMyChannel = currentUser == video.UploaderId;
            ViewBag.IsSubscriber = await userService.IsSubscriberAsync(id, currentUser);

            Response.SetCache(key, false, "id");

            return View(video);
        }

        // GET: Video/Thumbnail/id
        [HttpGet]
        [ChildActionOnly]
        public async Task<ActionResult> Thumbnail(string id)
        {
            string key = CacheKeys.VideoCacheKey(id);
            VideoProxy video = await Redis.GetCachedAsync(key, async () => await userService.GetVideoAsync(id));
            Response.SetCache(key, false, "id");
            return PartialView("_VideoThumbnail", video);
        }

        // GET: Video/SimilarThumbnail/id
        [HttpGet]
        [ChildActionOnly]
        public async Task<ActionResult> SimilarThumbnail(string id)
        {
            string key = CacheKeys.VideoCacheKey(id);
            VideoProxy video = await Redis.GetCachedAsync(key, async () => await userService.GetVideoAsync(id));
            Response.SetCache(key, false, "id");
            return PartialView("_SimilarVideoThumbnail", video);
        }

        // GET: Video/Similar/id
        public async Task<ActionResult> Similar(string id)
        {
            string videoKey = CacheKeys.VideoCacheKey(id);
            string similarVideosKey = CacheKeys.SimilarVideosCacheKey();

            VideoProxy video = await Redis.GetCachedAsync(videoKey, async () => await userService.GetVideoAsync(id));
            var similarVideos = await Redis.GetCachedWithParamAsync(
                similarVideosKey, id, async () => 
                {
                    var result = await userService.GetSimilarVideosAsync(video, 0, 10);
                    return result.Select(v => v.Id);
                });

            Response.SetCache(videoKey, false, "id");
            Response.SetCache(similarVideosKey, false, "id");

            return PartialView(similarVideos);
        }

        // GET: Video/Comments/parametr/page
        public async Task<ActionResult> Comments(string parametr, int page)
        {
            string key = CacheKeys.VideoCommentsCacheKey(parametr);

            Tuple<IEnumerable<CommentDTO>, long> tuple = await Redis.GetCachedPageAsync(
                key, page, async () =>
                {
                    var comments = await userService.GetCommentsAsync(parametr, (page - 1) * commentsOnPage, commentsOnPage);
                    long commentCount = await userService.GetCommentsCountAsync(parametr);
                    return new Tuple<IEnumerable<CommentDTO>, long>(comments, commentCount);
                });

            ViewBag.CommentCount = tuple.Item2;
            ViewBag.PageCount = Math.Ceiling(1.0 * tuple.Item2 / commentsOnPage);
            ViewBag.Page = page;
            ViewBag.Action = "Comments";
            ViewBag.Controller = "Video";
            ViewBag.Parametr = parametr;

            Response.SetCache(key, false, "parametr", "page");

            return PartialView(tuple.Item1);
        }

        // POST: Video/AddComment/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment(string id, string comment)
        {
            string key = CacheKeys.VideoCommentsCacheKey(id);

            Redis.DeleteKeyAsync(key);

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
        public async Task<ActionResult> Estimate(string id, int status)
        {
            string key = CacheKeys.VideoCacheKey(id);

            VideoProxy video = await Redis.GetCachedAsync(key, async () => await userService.GetVideoAsync(id));

            ViewedVideoTransferDTO transfer = new ViewedVideoTransferDTO
            {
                Status = (ViewStatus)status,
                ViewedVideo = id,
                Viewer = User.Identity.GetUserId(),
                ShowDateTime = DateTimeOffset.Now,
            };

            bool result = await userService.EstimateVideoAsync(transfer, video);
            if (result)
            {
                Redis.UpdateCacheAsync(key, video);
            }

            return Json(new { status = "OK" });
        }
    }
}
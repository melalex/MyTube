using Microsoft.AspNet.Identity;
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
        public async Task<ActionResult> UploadVideo(VideoUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Task.Run(async () =>
                //{
                //    string videoPath = FileUpload.UploadFile(model.Video);
                //    string posterPath = FileUpload.UploadFile(model.Poster);
                //    string channelId = User.Identity.GetUserId();
                //    await userService.CreateVideoAsync(
                //        channelId, 
                //        model.Name, 
                //        model.Description, 
                //        model.Category.SelectedValue.ToString(),
                //        model.TagsList,
                //        videoPath,
                //        posterPath
                //        );
                //    FileUpload.RemoveFile(videoPath);
                //    FileUpload.RemoveFile(posterPath);
                //});

                string videoPath = FileUpload.UploadFile(model.Video);
                string posterPath = FileUpload.UploadFile(model.Poster);
                string channelId = User.Identity.GetUserId();
                await userService.CreateVideoAsync(
                    channelId,
                    model.Name,
                    model.Description,
                    model.SelectedCategory,
                    model.TagsList,
                    videoPath,
                    posterPath
                    );
                FileUpload.RemoveFile(videoPath);
                FileUpload.RemoveFile(posterPath);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
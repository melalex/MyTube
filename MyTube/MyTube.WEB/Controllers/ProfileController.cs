using MyTube.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using MyTube.BLL.BusinessEntities;
using MyTube.BLL.Identity.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using MyTube.WEB.Models.Profile;
using MyTube.WEB.Models;
using MyTube.WEB.Models.Caching;

namespace MyTube.WEB.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private IUserService userService;
        private IIdentityService _identityService;

        public ProfileController(IUserService userService)
        {
            this.userService = userService;
        }

        public IIdentityService ApplicationIdentityService
        {
            get
            {
                return _identityService ?? HttpContext.GetOwinContext().Get<IIdentityService>();
            }
            private set
            {
                _identityService = value;
            }
        }
         
        // GET: Profile/Edit
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit()
        {
            string channelId = User.Identity.GetUserId();
            string channelKey = CacheKeys.EditChannelCacheKey(channelId);

            var editProfileViewModel = await Redis.GetCachedAsync(
                channelKey, async () =>
                {
                    var user = await ApplicationIdentityService.FindByIdAsync(channelId);
                    var channel = await userService.GetChannelAsync(channelId);
                    return new EditProfileViewModel(user, channel);
                }); 

            Response.SetCache(channelKey, true);

            return View(editProfileViewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProfileViewModel editProfileViewModel)
        {
            ActionResult result = null;
            if (ModelState.IsValid)
            {
                string channelId = User.Identity.GetUserId();
                var editResult = await ApplicationIdentityService.EditUserAsync(
                    channelId,
                    editProfileViewModel.Email,
                    editProfileViewModel.Username,
                    editProfileViewModel.NewPassword,
                    editProfileViewModel.Password
                    );

                if (editResult.Succeeded)
                {
                    string channelKey = CacheKeys.ChannelCacheKey(channelId);

                    var channel = await Redis.GetCachedAsync(
                        channelKey, async () => await userService.GetChannelAsync(channelId)
                        );

                    await userService.EditChannelUsernameAsync(channel, editProfileViewModel.Username);

                    await Redis.UpdateCacheAsync(channelKey, channel);
                    await Redis.DeleteKeyAsync(CacheKeys.EditChannelCacheKey(channelId));
                    await Redis.DeleteKeyAsync(CacheKeys.ChannelThumbnailCacheKey(channelId));

                    await userService.ForEachVideoFromChannelAsync(
                        channelId, async v => 
                        {
                            await Redis.DeleteKeyAsync(CacheKeys.VideoCacheKey(v.Id));
                        });

                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", editResult.Errors.FirstOrDefault());
                    result = View(editProfileViewModel);
                }
            }
            else
            {
                result = View(editProfileViewModel);
            }

            return result;
        }

        // POST: Profile/EditAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAvatar(HttpPostedFileBase newAvatar)
        {
            ActionResult result = null;
            if (ModelState.IsValid)
            {
                string path = FileUpload.UploadFile(newAvatar);
                if (path != null)
                {
                    string channelId = User.Identity.GetUserId();
                    string channelKey = CacheKeys.ChannelCacheKey(channelId);

                    var channel = await Redis.GetCachedAsync(
                        channelKey, async () => await userService.GetChannelAsync(channelId)
                        );

                    await userService.EditChannelAvatarAsync(channel, path);
                    await Redis.UpdateCacheAsync(channelKey, channel);

                    await userService.ForEachVideoFromChannelAsync(
                        channelId, async v =>
                        {
                            await Redis.DeleteKeyAsync(CacheKeys.VideoCacheKey(v.Id));
                        });
                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Image", "User invalid");
                    result = RedirectToAction("Edit");
                }
                FileUpload.RemoveFile(path);
            }
            else
            {
                result = RedirectToAction("Edit");
            }

            return result;
        }

        
        // Get: Profile/AuthenticatedProfileZone/id
        [ChildActionOnly]
        public ActionResult AuthenticatedProfileZone()
        {
            string channelId = User.Identity.GetUserId();
            string thumbnailCacheKey = CacheKeys.ChannelCacheKey(channelId);
            var channel = Redis.GetCached(thumbnailCacheKey, () =>
            {
                return Task.Run(() => userService.GetChannelAsync(channelId)).Result;
            });

            return PartialView("_AuthenticatedProfileZone", channel);
        }
    }
}
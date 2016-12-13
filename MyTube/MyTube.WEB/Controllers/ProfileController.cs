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
        public async Task<ActionResult> Edit()
        {
            var channelId = User.Identity.GetUserId();
            var user = await ApplicationIdentityService.FindByIdAsync(channelId);
            var channel = await userService.GetChannelAsync(channelId);
            var editProfileViewModel = new EditProfileViewModel(user, channel);
            return View(editProfileViewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProfileViewModel editProfileViewModel)
        {
            ActionResult result = null;
            if (ModelState.IsValid)
            {
                var editResult = await ApplicationIdentityService.EditUserAsync(
                    User.Identity.GetUserId(),
                    editProfileViewModel.Email,
                    editProfileViewModel.Username,
                    editProfileViewModel.NewPassword,
                    editProfileViewModel.Password
                    );

                if (editResult.Succeeded)
                {
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

                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Image", "User invalid");
                    result = RedirectToAction("Edit");
                }
            }
            else
            {
                result = RedirectToAction("Edit");
            }

            return result;
        }

        // Get: Profile/AuthenticatedProfileZone/id
        public ActionResult AuthenticatedProfileZone()
        {
            var channelId = User.Identity.GetUserId();
            var channel = Task.Run(() => userService.GetChannelAsync(channelId)).Result;
            return PartialView("_AuthenticatedProfileZone", channel);
        }
    }
}
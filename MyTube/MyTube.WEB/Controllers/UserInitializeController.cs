using Microsoft.AspNet.Identity.Owin;
using MyTube.BLL.Identity.DTO;
using MyTube.BLL.Identity.Interfaces;
using MyTube.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Controllers
{
    public class UserInitializeController : Controller
    {
        private IIdentityService _identityService;
        private IUserService userService;

        public UserInitializeController(IUserService userService)
        {
            this.userService = userService;
        }

        public UserInitializeController(IIdentityService identityService, IUserService userService)
        {
            ApplicationIdentityService = identityService;
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

        // GET: UserInitialize
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: UserInitialize
        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase upload)
        {
            string line = null;
            string[] colums = null;
            using (var reader = new StreamReader(upload.InputStream))
            {
                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();
                    colums = line.Split(',');

                    var channelId = await userService.CreateChannelAsync(colums[0]);
                    var user = new UserDTO { Id = channelId, Username = colums[0], Email = colums[1] };
                    var result = await ApplicationIdentityService.CreateAsync(user, colums[2]);

                    if (!result.Succeeded)
                    {
                        await userService.DeleteChannelAsync(channelId);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
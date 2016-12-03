using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using MyTube.DAL.Identity.Entities;
using MyTube.DAL.Identity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Identity.Services
{
    static public class IdentityValidator
    {
        static public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)
                        );
        }
    }
}

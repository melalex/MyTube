using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MyTube.DAL.Identity.Entities;
using MyTube.DAL.Identity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Identity.Infrastructure
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public async Task<SignInStatus> PasswordEmailSignInAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            else
            {
                return await PasswordSignInAsync(user.UserName, password, isPersistent, shouldLockout);
            }
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
    }
}

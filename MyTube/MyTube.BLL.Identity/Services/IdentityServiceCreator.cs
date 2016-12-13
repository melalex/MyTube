using MyTube.BLL.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MyTube.BLL.Identity.Infrastructure;
using MyTube.DAL.Identity.Repositories;
using MyTube.DAL.Identity.Identity;
using Microsoft.AspNet.Identity;
using MyTube.DAL.Identity.Entities;

namespace MyTube.BLL.Identity.Services
{
    public class IdentityServiceCreator : IIdentityServiceCreator
    {
        public IIdentityService Create(string connectioString, IOwinContext context)
        {
            IdentityUnitOfWork unitOfWork = new IdentityUnitOfWork(connectioString);
            ApplicationSignInManager signInManager = new ApplicationSignInManager(unitOfWork.UserManager, context.Authentication);
            configureUserManager(unitOfWork.UserManager);
            return new IdentityService(unitOfWork, signInManager);
        }

        private void configureUserManager(ApplicationUserManager manager)
        {
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true,
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
        }
    }
}

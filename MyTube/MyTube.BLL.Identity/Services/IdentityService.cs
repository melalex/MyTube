using MyTube.BLL.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using MyTube.BLL.Identity.DTO;
using MyTube.DAL.Identity.Interfaces;
using MyTube.BLL.Identity.Infrastructure;
using AutoMapper;
using MyTube.DAL.Identity.Entities;
using MyTube.DAL.Identity.Identity;

namespace MyTube.BLL.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private IIdentityUnitOfWork database;
        private ApplicationSignInManager signInManager;

        public IdentityService(IIdentityUnitOfWork database, ApplicationSignInManager signInManager)
        {
            this.database = database;
            this.signInManager = signInManager;

            Mapper.Initialize(cfg => cfg.CreateMap<ApplicationUser, UserDTO>());
            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, ApplicationUser>());
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            return await database.UserManager.ConfirmEmailAsync(userId, token);
        }

        public async Task<IdentityResult> CreateAsync(UserDTO user, string password)
        {
            ApplicationUser appUser = new ApplicationUser();
            Mapper.Map(user, appUser);
            return await database.UserManager.CreateAsync(appUser, password);
        }

        public async Task<UserDTO> FindByEmailAsync(string email)
        {
            ApplicationUser appUser = await database.UserManager.FindByEmailAsync(email);
            UserDTO user = new UserDTO();
            Mapper.Map(appUser, user);
            return user;
        }

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            return await database.UserManager.IsEmailConfirmedAsync(userId);
        }

        public async Task<SignInStatus> PasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            return await signInManager.PasswordEmailSignInAsync(email, password, isPersistent, shouldLockout);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            return await database.UserManager.ResetPasswordAsync(userId, token, newPassword);
        }

        public async Task SignInAsync(UserDTO user, bool isPersistent, bool rememberBrowser)
        {
            ApplicationUser appUser = new ApplicationUser();
            Mapper.Map(user, appUser);
            await signInManager.SignInAsync(appUser, isPersistent, rememberBrowser);
        }

        public void Dispose()
        {
            database.Dispose();
            signInManager.Dispose();
        }
    }
}

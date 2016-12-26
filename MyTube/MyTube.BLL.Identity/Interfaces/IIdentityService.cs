using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using MyTube.BLL.Identity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Identity.Interfaces
{
    public interface IIdentityService : IDisposable
    {
        Task<SignInStatus> PasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout);
        Task SignInAsync(UserDTO user, bool isPersistent, bool rememberBrowser);

        Task<IdentityResult> CreateAsync(UserDTO user, string password);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<UserDTO> FindByEmailAsync(string email);
        Task<UserDTO> FindByIdAsync(string id);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<IdentityResult> EditUserAsync(string userId, string newEmail, string newUsername, string newPassword, string oldPassword);
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        void BecomeAdmin(string id);
    }
}

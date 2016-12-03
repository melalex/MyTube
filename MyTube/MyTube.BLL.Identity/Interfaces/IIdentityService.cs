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
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
        Task SignInAsync(UserDTO user, bool isPersistent, bool rememberBrowser);

        Task<IdentityResult> CreateAsync(UserDTO user, string password);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<UserDTO> FindByNameAsync(string name);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        Func<CookieValidateIdentityContext, Task> OnValidateIdentity();
    }
}

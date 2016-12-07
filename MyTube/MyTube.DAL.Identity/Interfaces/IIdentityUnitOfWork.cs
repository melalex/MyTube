using MyTube.DAL.Identity.Entities;
using MyTube.DAL.Identity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Identity.Interfaces
{
    public interface IIdentityUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }
        Task SaveAsync();
        Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName, int skip, int limit);
        Task<int> GetCountOfUsersByRole(string roleName);
    }
}

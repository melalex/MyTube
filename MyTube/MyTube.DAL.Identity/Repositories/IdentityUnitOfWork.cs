using Microsoft.AspNet.Identity.EntityFramework;
using MyTube.DAL.Identity.EF;
using MyTube.DAL.Identity.Entities;
using MyTube.DAL.Identity.Identity;
using MyTube.DAL.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MyTube.DAL.Identity.Repositories
{
    public class IdentityUnitOfWork : IIdentityUnitOfWork
    {
        private ApplicationDbContext database;

        private ApplicationUserManager userManager = null;
        private ApplicationRoleManager roleManager = null;

        public IdentityUnitOfWork(string connectionString)
        {
            database = new ApplicationDbContext(connectionString);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName, int skip, int limit)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            return await database.Users
                .Where(u => u.Roles.Select(r => r.RoleId)
                .Contains(role.Id))
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetCountOfUsersByRole(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            return await database.Users
                .Where(u => u.Roles.Select(r => r.RoleId)
                .Contains(role.Id))
                .CountAsync();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (userManager == null)
                {
                    userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(database));
                }
                return userManager;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                if (roleManager == null)
                {
                    roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(database));
                }
                return roleManager;
            }
        }

        public async Task SaveAsync()
        {
            await database.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager?.Dispose();
                    roleManager?.Dispose();
                }
                this.disposed = true;
            }
        }
    }
}

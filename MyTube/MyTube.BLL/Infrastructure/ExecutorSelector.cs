using MyTube.DAL.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTube.BLL.Infrastructure
{
    internal class ExecutorSelector
    {
        private const string moderatorRole = "moderator";
        private static int previosExecutor = -1;

        private IIdentityUnitOfWork database;

        internal ExecutorSelector(IIdentityUnitOfWork database)
        {
            this.database = database;
        }

        public async Task<string> selectExecutorAsync()
        {
            int currentExecutor = Interlocked.Increment(ref previosExecutor);
            int moderatorsCount = await database.GetCountOfUsersByRole(moderatorRole);
            var moderators = await database.GetUsersIdsByRole(moderatorRole, currentExecutor % moderatorsCount, 1);
            return moderators.FirstOrDefault();
        }
    }
}
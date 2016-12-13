using MyTube.DAL.FileStorage;
using MyTube.DAL.FileStorage.Interfaces;
using MyTube.DAL.Identity.Interfaces;
using MyTube.DAL.Identity.Repositories;
using MyTube.DAL.Interfaces;
using MyTube.DAL.Repositories;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        private string mongoConnectionString;
        private string identityConnectionString;

        public ServiceModule(string mongoConnection, string identityConnection)
        {
            mongoConnectionString = mongoConnection;
            identityConnectionString = identityConnection;
        }
        public override void Load()
        {
            Bind<IUnitOfWork>().To<MongoUnitOfWork>().WithConstructorArgument(mongoConnectionString);
            Bind<IIdentityUnitOfWork>().To<IdentityUnitOfWork>().WithConstructorArgument(identityConnectionString);
            Bind<IStorageFacade>().To<LocalFileStorageFacade>();
        }
    }
}
